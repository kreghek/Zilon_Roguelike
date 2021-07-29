using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.GameObjectVisualization;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.VisualEffects;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal class ActorViewModel : GameObjectBase, IActorViewModel
    {
        private readonly IList<IActorStateEngine> _actorStateEngineList;
        private readonly Game _game;
        private readonly IGameObjectVisualizationContentStorage _gameObjectVisualizationContentStorage;
        private readonly IActorGraphics _graphicsRoot;
        private readonly IPersonSoundContentStorage _personSoundStorage;
        private readonly SpriteContainer _rootSprite;
        private readonly SectorViewModelContext _sectorViewModelContext;
        private readonly Sprite _shadowSprite;
        private readonly SpriteBatch _spriteBatch;

        public ActorViewModel(
            IActor actor,
            GameObjectParams gameObjectParams)
        {
            Actor = actor;

            _game = gameObjectParams.Game ??
                    throw new ArgumentException($"{nameof(gameObjectParams.Game)} is not defined.",
                        nameof(gameObjectParams));
            _sectorViewModelContext = gameObjectParams.SectorViewModelContext ??
                                      throw new ArgumentException(
                                          $"{nameof(gameObjectParams.SectorViewModelContext)} is not defined.",
                                          nameof(gameObjectParams));
            _personSoundStorage = gameObjectParams.PersonSoundStorage ??
                                  throw new ArgumentException(
                                      $"{nameof(gameObjectParams.PersonSoundStorage)} is not defined.",
                                      nameof(gameObjectParams));

            _gameObjectVisualizationContentStorage = gameObjectParams.GameObjectVisualizationContentStorage ??
                                                     throw new ArgumentException(
                                                         $"{nameof(gameObjectParams.GameObjectVisualizationContentStorage)} is not defined.",
                                                         nameof(gameObjectParams));

            _spriteBatch = gameObjectParams.SpriteBatch ??
                           throw new ArgumentException($"{nameof(gameObjectParams.SpriteBatch)} is not defined.",
                               nameof(gameObjectParams));

            if (gameObjectParams.PersonVisualizationContentStorage is null)
            {
                throw new ArgumentException(
                    $"{nameof(gameObjectParams.PersonVisualizationContentStorage)} is not defined.",
                    nameof(gameObjectParams));
            }

            var equipmentModule = Actor.Person.GetModuleSafe<IEquipmentModule>();

            var shadowTexture = _game.Content.Load<Texture2D>("Sprites/game-objects/simple-object-shadow");

            _rootSprite = new SpriteContainer();
            _shadowSprite = new Sprite(shadowTexture)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.5f),
                Color = new Color(Color.White, 0.5f)
            };

            _rootSprite.AddChild(_shadowSprite);

            var isHumanGraphics = Actor.Person is HumanPerson;
            if (isHumanGraphics)
            {
                if (equipmentModule is not null)
                {
                    var graphicsRoot = new HumanoidGraphics(equipmentModule,
                        gameObjectParams.PersonVisualizationContentStorage);

                    _rootSprite.AddChild(graphicsRoot);

                    _graphicsRoot = graphicsRoot;
                }
                else
                {
                    // There is no cases when human person hasn't equipment module.
                    // It can be empty module to show "naked" person in the future.
                    throw new InvalidOperationException("Person has no IEquipmentModule.");
                }
            }
            else
            {
                var monsterPerson = Actor.Person as MonsterPerson;
                SpriteContainer? graphics = null;
                switch (monsterPerson.Scheme.Sid)
                {
                    case "predator":
                        graphics = new AnimalGraphics(gameObjectParams.PersonVisualizationContentStorage);
                        break;

                    case "predator-water":
                        graphics = new AnimalGraphics(gameObjectParams.PersonVisualizationContentStorage);
                        break;

                    case "predator-meat":
                        graphics = new AnimalGraphics(gameObjectParams.PersonVisualizationContentStorage);
                        break;

                    case "predator-medkit":
                        graphics = new AnimalGraphics(gameObjectParams.PersonVisualizationContentStorage);
                        break;

                    case "predator-equipment":
                        graphics = new AnimalGraphics(gameObjectParams.PersonVisualizationContentStorage);
                        break;

                    case "gallbladder":
                        graphics = new MonoGraphics("gallbladder", gameObjectParams.PersonVisualizationContentStorage);
                        break;
                }

                _rootSprite.AddChild(graphics);

                _graphicsRoot = (IActorGraphics)graphics;
            }

            var hexSize = MapMetrics.UnitSize / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                playerActorWorldCoords[1] * hexSize * 2 / 2
            );

            _rootSprite.Position = newPosition;

            Actor.Moved += Actor_Moved;
            Actor.UsedProp += Actor_UsedProp;
            Actor.PropTransferPerformed += Actor_PropTransferPerformed;
            Actor.BeginTransitionToOtherSector += Actor_BeginTransitionToOtherSector;
            if (Actor.Person.HasModule<IEquipmentModule>())
            {
                Actor.Person.GetModule<IEquipmentModule>().EquipmentChanged += PersonEquipmentModule_EquipmentChanged;
            }

            if (Actor.Person.HasModule<ISurvivalModule>())
            {
                Actor.Person.GetModule<ISurvivalModule>().Dead += PersonSurvivalModule_Dead;
            }

            _actorStateEngineList = new List<IActorStateEngine> { new ActorIdleEngine(_graphicsRoot.RootSprite) };
        }

        public override bool HiddenByFow => true;

        public override Vector2 HitEffectPosition => _graphicsRoot.HitEffectPosition;

        public bool IsGraphicsOutlined { get; set; }
        public override IGraphNode Node => Actor.Node;

        public override void Draw(GameTime gameTime, Matrix transform)
        {
            _spriteBatch.Begin(transformMatrix: transform);

            _rootSprite.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public override void HandleRemove()
        {
            base.HandleRemove();

            foreach (var state in _actorStateEngineList)
            {
                state.Cancel();
            }
        }

        public void RunCombatActUsageAnimation(ActDescription usedActDescription, IGraphNode targetNode)
        {
            if (!CanDraw)
            {
                return;
            }

            var serviceScope = ((LivGame)_game).ServiceProvider;

            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

            var hexSize = MapMetrics.UnitSize / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)targetNode).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                playerActorWorldCoords[1] * hexSize * 2 / 2
            );

            var targetSpritePosition = newPosition;

            var attackSoundEffectInstance = GetAttackSoundEffect(usedActDescription);
            var hitVisualEffect = GetAttackVisualEffect(targetNode, targetSpritePosition, usedActDescription);

            var stateEngine = new ActorMeleeAttackEngine(
                _rootSprite,
                targetSpritePosition,
                animationBlockerService,
                attackSoundEffectInstance,
                hitVisualEffect);
            AddStateEngine(stateEngine);
        }

        public void RunDamageReceivedAnimation(IGraphNode attackerNode)
        {
            var serviceScope = ((LivGame)_game).ServiceProvider;
            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

            var soundEffectInstance = GetPersonImpactSoundEffect(Actor.Person);

            var hexSize = MapMetrics.UnitSize / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)attackerNode).OffsetCoords);
            var attackerPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                playerActorWorldCoords[1] * hexSize * 2 / 2
            );

            var moveEngine = new ActorDamagedEngine(_graphicsRoot, _rootSprite, attackerPosition,
                animationBlockerService,
                soundEffectInstance);

            AddStateEngine(moveEngine);
        }

        public void RunDeathAnimation()
        {
            var deathSoundEffect = _personSoundStorage.GetDeathEffect(Actor.Person);
            var soundEffectInstance = deathSoundEffect.CreateInstance();

            _rootSprite.RemoveChild(_graphicsRoot.RootSprite);

            var corpse = new CorpseViewModel(_game, _graphicsRoot, _rootSprite.Position, soundEffectInstance);
            _sectorViewModelContext.CorpseManager.Add(corpse);
        }

        public void UnsubscribeEventHandlers()
        {
            Actor.Moved -= Actor_Moved;
            Actor.UsedProp -= Actor_UsedProp;
            Actor.PropTransferPerformed -= Actor_PropTransferPerformed;
            Actor.BeginTransitionToOtherSector -= Actor_BeginTransitionToOtherSector;

            if (Actor.Person.HasModule<IEquipmentModule>())
            {
                Actor.Person.GetModule<IEquipmentModule>().EquipmentChanged -= PersonEquipmentModule_EquipmentChanged;
            }

            if (Actor.Person.HasModule<ISurvivalModule>())
            {
                Actor.Person.GetModule<ISurvivalModule>().Dead -= PersonSurvivalModule_Dead;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_actorStateEngineList.Any())
            {
                var activeStateEngine = _actorStateEngineList.First();
                activeStateEngine.Update(gameTime);

                if (activeStateEngine.IsComplete)
                {
                    _actorStateEngineList.Remove(activeStateEngine);

                    if (!_actorStateEngineList.Any())
                    {
                        AddStateEngine(new ActorIdleEngine(_graphicsRoot.RootSprite));
                    }

                    ResetActorRootSpritePosition();
                }
            }

            var keyboard = Keyboard.GetState();
            _graphicsRoot.ShowOutlined = keyboard.IsKeyDown(Keys.LeftAlt) || IsGraphicsOutlined;
        }

        private void Actor_BeginTransitionToOtherSector(object? sender, EventArgs e)
        {
            var serviceScope = ((LivGame)_game).ServiceProvider;
            var player = serviceScope.GetRequiredService<IPlayer>();
            if (sender is IActor actor && actor.Person != player.MainPerson)
            {
                // Do not sound transition of monsters or npc now.
                // In the client are no sounds to this.
                return;
            }

            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();
            var soundEffect = _personSoundStorage.GetActivitySound(PersonActivityEffectType.Transit);
            var stateEngine = new ActorSectorTransitionMoveEngine(
                _graphicsRoot.RootSprite,
                animationBlockerService,
                soundEffect?.CreateInstance());

            AddStateEngine(stateEngine);
        }

        private void Actor_Moved(object? sender, EventArgs e)
        {
            var hexSize = MapMetrics.UnitSize / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                playerActorWorldCoords[1] * hexSize * 2 / 2
            );

            if (!CanDraw)
            {
                _rootSprite.Position = newPosition;

                return;
            }

            var serviceScope = ((LivGame)_game).ServiceProvider;

            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

            SoundEffectInstance? moveSoundEffectInstance = null;

            var player = serviceScope.GetRequiredService<IPlayer>();
            if (sender is IActor actor && actor.Person == player.MainPerson)
            {
                // Sound steps of main person only to prevent infinite steps loop.
                var moveSoundEffect = _personSoundStorage.GetActivitySound(PersonActivityEffectType.Move);
                moveSoundEffectInstance = moveSoundEffect?.CreateInstance();
            }

            var moveEngine = new ActorMoveEngine(
                _rootSprite,
                _graphicsRoot.RootSprite,
                _shadowSprite,
                newPosition,
                animationBlockerService,
                moveSoundEffectInstance);

            AddStateEngine(moveEngine);
        }

        private void Actor_PropTransferPerformed(object? sender, EventArgs e)
        {
            var serviceScope = ((LivGame)_game).ServiceProvider;
            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();
            var soundEffect = _personSoundStorage.GetActivitySound(PersonActivityEffectType.Transit);
            var stateEngine = new ActorCommonActionMoveEngine(
                _graphicsRoot.RootSprite,
                animationBlockerService,
                soundEffect?.CreateInstance());

            AddStateEngine(stateEngine);
        }

        private void Actor_UsedProp(object? sender, UsedPropEventArgs e)
        {
            var serviceScope = ((LivGame)_game).ServiceProvider;
            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();
            var visualizationContentStorage = serviceScope.GetRequiredService<IGameObjectVisualizationContentStorage>();

            var consumableType = e.UsedProp.Scheme.Sid switch
            {
                "med-kit" => ConsumeEffectType.Heal,
                "water-bottle" => ConsumeEffectType.Drink,
                "packed-food" => ConsumeEffectType.Eat,
                _ => ConsumeEffectType.UseCommon
            };

            var soundEffect = _personSoundStorage.GetConsumePropSound(consumableType);

            var stateEngine = new ActorCommonActionMoveEngine(_graphicsRoot.RootSprite, animationBlockerService,
                soundEffect?.CreateInstance());

            AddStateEngine(stateEngine);

            var hexSize = MapMetrics.UnitSize / 2;
            var actorNode = (HexNode)(Actor.Node);
            var playerActorWorldCoords = HexHelper.ConvertToWorld(actorNode.OffsetCoords.X, actorNode.OffsetCoords.Y);
            var actorPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                playerActorWorldCoords[1] * hexSize * 2 / 2
            );

            const int START_EFFECT_Y = 24;
            var consumeEffect = new ConsumingEffect(
                visualizationContentStorage,
                actorPosition - (Vector2.UnitY * START_EFFECT_Y),
                this,
                consumableType
            );
            _sectorViewModelContext.EffectManager.VisualEffects.Add(consumeEffect);
        }

        private void AddStateEngine(IActorStateEngine actorStateEngine)
        {
            foreach (var state in _actorStateEngineList.ToArray())
            {
                if (state.CanBeReplaced)
                {
                    _actorStateEngineList.Remove(state);
                }
            }

            _actorStateEngineList.Add(actorStateEngine);
        }

        private SoundEffectInstance? GetAttackSoundEffect(ActDescription usedActDescription)
        {
            var attackSoundEffect = _personSoundStorage.GetActStartSound(usedActDescription);

            var attackSoundEffectInstance = attackSoundEffect?.CreateInstance();
            return attackSoundEffectInstance;
        }

        private IVisualEffect? GetAttackVisualEffect(IGraphNode targetNode, Vector2 targetSpritePosition,
            ActDescription usedActDescription)
        {
            // Selection actors only is prevention of a error when a monster stays on a loot bag.
            var actorViewModels = _sectorViewModelContext.GameObjects.Where(x => x is IActorViewModel);
            var targetGameObject = actorViewModels.SingleOrDefault(x => x.Node == targetNode);

            if (targetGameObject is null)
            {
                // This means the attacker missed.
                // This situation can be then the target actor moved before the attack reaches the target.
                return null;
            }

            var difference = targetSpritePosition - _rootSprite.Position;
            var hitEffectPosition = (difference / 2) + _rootSprite.Position + HitEffectPosition;
            var direction = difference;
            direction.Normalize();

            var hitEffect = new HitEffect(this, targetGameObject, _gameObjectVisualizationContentStorage,
                hitEffectPosition, direction, usedActDescription);
            _sectorViewModelContext.EffectManager.VisualEffects.Add(hitEffect);

            return hitEffect;
        }

        private static string[] GetClearTags(Equipment? equipment)
        {
            return equipment?.Scheme.Tags?.Where(x => x != null)?.Select(x => x!)?.ToArray() ??
                   Array.Empty<string>();
        }

        private SoundEffectInstance GetPersonImpactSoundEffect(IPerson person)
        {
            if (person.CheckIsDead())
            {
                // Dead person is not suffer pain.
                // Looks like error.
                Debug.Fail("");
            }

            var impactSoundEffect = _personSoundStorage.GetImpactEffect(person);
            return impactSoundEffect.CreateInstance();
        }

        private void PersonEquipmentModule_EquipmentChanged(object? sender, EquipmentChangedEventArgs e)
        {
            var serviceScope = ((LivGame)_game).ServiceProvider;
            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

            var equipment = e.Equipment;
            var soundSoundEffect = SelectEquipEffect(equipment);

            var stateEngine = new ActorCommonActionMoveEngine(_graphicsRoot.RootSprite, animationBlockerService,
                soundSoundEffect?.CreateInstance());

            AddStateEngine(stateEngine);
        }

        private void PersonSurvivalModule_Dead(object? sender, EventArgs e)
        {
            RunDeathAnimation();
        }

        private void ResetActorRootSpritePosition()
        {
            var hexSize = MapMetrics.UnitSize / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                playerActorWorldCoords[1] * hexSize * 2 / 2
            );

            _rootSprite.Position = newPosition;
        }

        private SoundEffect? SelectEquipEffect(Equipment? equipment)
        {
            var clearTags = GetClearTags(equipment);
            return _personSoundStorage.GetEquipSound(clearTags, equipment != null);
        }

        public IActor Actor { get; set; }
        public object Item => Actor;
    }
}