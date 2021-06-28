using System;
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
        private readonly Game _game;
        private readonly IActorGraphics _graphicsRoot;
        private readonly IPersonSoundContentStorage _personSoundStorage;

        private readonly SpriteContainer _rootSprite;
        private readonly SectorViewModelContext _sectorViewModelContext;
        private readonly Sprite _shadowSprite;
        private readonly SpriteBatch _spriteBatch;

        private IActorStateEngine _actorStateEngine;

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
                var graphicsRoot = new AnimalGraphics(gameObjectParams.PersonVisualizationContentStorage);

                _rootSprite.AddChild(graphicsRoot);

                _graphicsRoot = graphicsRoot;
            }

            var hexSize = MapMetrics.UnitSize / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                playerActorWorldCoords[1] * hexSize * 2 / 2
            );

            _rootSprite.Position = newPosition;

            Actor.Moved += Actor_Moved;
            Actor.UsedAct += Actor_UsedAct;
            Actor.DamageTaken += Actor_DamageTaken;
            Actor.UsedProp += Actor_UsedProp;
            Actor.PropTransferPerformed += Actor_PropTransferPerformed;
            Actor.BeginTransitionToOtherSector += Actor_BeginTransitionToOtherSector;
            if (Actor.Person.HasModule<IEquipmentModule>())
            {
                Actor.Person.GetModule<IEquipmentModule>().EquipmentChanged += Actor_EquipmentChanged;
            }

            _actorStateEngine = new ActorIdleEngine(_graphicsRoot.RootSprite);
        }

        public override bool HiddenByFow => true;

        public override Vector2 HitEffectPosition => _graphicsRoot.HitEffectPosition;
        public override IGraphNode Node => Actor.Node;

        public override void Draw(GameTime gameTime, Matrix transform)
        {
            _spriteBatch.Begin(transformMatrix: transform);

            _rootSprite.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public void UnsubscribeEventHandlers()
        {
            Actor.Moved -= Actor_Moved;
            Actor.UsedAct -= Actor_UsedAct;
            Actor.DamageTaken -= Actor_DamageTaken;
            Actor.UsedProp -= Actor_UsedProp;
            Actor.PropTransferPerformed -= Actor_PropTransferPerformed;
            Actor.BeginTransitionToOtherSector -= Actor_BeginTransitionToOtherSector;

            if (Actor.Person.HasModule<IEquipmentModule>())
            {
                Actor.Person.GetModule<IEquipmentModule>().EquipmentChanged -= Actor_EquipmentChanged;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_actorStateEngine != null)
            {
                _actorStateEngine.Update(gameTime);
                if (_actorStateEngine.IsComplete)
                {
                    _actorStateEngine = new ActorIdleEngine(_graphicsRoot.RootSprite);

                    var hexSize = MapMetrics.UnitSize / 2;
                    var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
                    var newPosition = new Vector2(
                        (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                        playerActorWorldCoords[1] * hexSize * 2 / 2
                    );

                    _rootSprite.Position = newPosition;
                }
            }

            var keyboard = Keyboard.GetState();
            _graphicsRoot.ShowOutlined = keyboard.IsKeyDown(Keys.LeftAlt);
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
            _actorStateEngine = new ActorSectorTransitionMoveEngine(
                _graphicsRoot.RootSprite,
                animationBlockerService,
                soundEffect?.CreateInstance());
        }

        private void Actor_DamageTaken(object? sender, DamageTakenEventArgs e)
        {
            if (sender is not Actor actor)
            {
                Debug.Fail("Sender must be IActor");
                // Do nothing. Looks like error. But client must not down.
                return;
            }

            if (actor.Person.CheckIsDead())
            {
                var deathSoundEffect = _personSoundStorage.GetDeathEffect(actor.Person);
                deathSoundEffect.CreateInstance().Play();
            }
            else
            {
                var impactSoundEffect = _personSoundStorage.GetImpactEffect(actor.Person);
                impactSoundEffect.CreateInstance().Play();
            }
        }

        private void Actor_EquipmentChanged(object? sender, EquipmentChangedEventArgs e)
        {
            var serviceScope = ((LivGame)_game).ServiceProvider;
            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

            var equipment = e.Equipment;
            var soundSoundEffect = SelectEquipEffect(equipment);

            _actorStateEngine = new ActorCommonActionMoveEngine(_graphicsRoot.RootSprite, animationBlockerService,
                soundSoundEffect?.CreateInstance());
        }

        private void Actor_Moved(object? sender, EventArgs e)
        {
            var hexSize = MapMetrics.UnitSize / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                playerActorWorldCoords[1] * hexSize * 2 / 2
            );

            if (CanDraw)
            {
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
                _actorStateEngine = moveEngine;
            }
            else
            {
                _rootSprite.Position = newPosition;
            }
        }

        private void Actor_PropTransferPerformed(object? sender, EventArgs e)
        {
            var serviceScope = ((LivGame)_game).ServiceProvider;
            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();
            var soundEffect = _personSoundStorage.GetActivitySound(PersonActivityEffectType.Transit);
            _actorStateEngine = new ActorCommonActionMoveEngine(
                _graphicsRoot.RootSprite,
                animationBlockerService,
                soundEffect?.CreateInstance());
        }

        private void Actor_UsedAct(object? sender, UsedActEventArgs e)
        {
            var stats = e.TacticalAct.Stats;
            if (stats is null)
            {
                throw new InvalidOperationException("The act has no stats to select visualization.");
            }

            Debug.WriteLine(e.TacticalAct);

            if (CanDraw)
            {
                if (stats.Effect == TacticalActEffectType.Damage && stats.IsMelee)
                {
                    var serviceScope = ((LivGame)_game).ServiceProvider;

                    var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

                    var hexSize = MapMetrics.UnitSize / 2;
                    var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)e.TargetNode).OffsetCoords);
                    var newPosition = new Vector2(
                        (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                        playerActorWorldCoords[1] * hexSize * 2 / 2
                    );

                    var targetSpritePosition = newPosition;

                    var attackSoundEffectInstance = GetSoundEffect(e.TacticalAct.Stats);
                    _actorStateEngine =
                        new ActorMeleeAttackEngine(
                            _rootSprite,
                            targetSpritePosition,
                            animationBlockerService,
                            attackSoundEffectInstance);

                    // Selection actors only prevent error when monster stays on loot bag.
                    var actorViewModels = _sectorViewModelContext.GameObjects.Where(x => x is IActorViewModel);
                    var targetGameObject = actorViewModels.SingleOrDefault(x => x.Node == e.TargetNode);

                    if (targetGameObject is not null)
                    {
                        var direction = targetSpritePosition - _rootSprite.Position;
                        var effectPosition = targetSpritePosition + targetGameObject.HitEffectPosition;
                        var hitEffect = new HitEffect((LivGame)_game, effectPosition, direction);
                        _sectorViewModelContext.EffectManager.VisualEffects.Add(hitEffect);
                    }
                }
            }
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

            _actorStateEngine = new ActorCommonActionMoveEngine(_graphicsRoot.RootSprite, animationBlockerService,
                soundEffect?.CreateInstance());

            var hexSize = MapMetrics.UnitSize / 2;
            var actorNode = (HexNode)(Actor.Node);
            var playerActorWorldCoords = HexHelper.ConvertToWorld(actorNode.OffsetCoords.X, actorNode.OffsetCoords.Y);
            var actorPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                playerActorWorldCoords[1] * hexSize * 2 / 2
            );

            const int START_EFFECT_Y = 24;
            var consumeEffect = new ConsumingEffect(visualizationContentStorage,
                actorPosition - (Vector2.UnitY * START_EFFECT_Y),
                consumableType
            );
            _sectorViewModelContext.EffectManager.VisualEffects.Add(consumeEffect);
        }

        private static string[] GetClearTags(Equipment? equipment)
        {
            return equipment?.Scheme.Tags?.Where(x => x != null)?.Select(x => x!)?.ToArray() ??
                   Array.Empty<string>();
        }

        private SoundEffectInstance? GetSoundEffect(ITacticalActStatsSubScheme actStatScheme)
        {
            var usedActDescription = ActDescription.CreateFromActStats(actStatScheme);

            var attackSoundEffect = _personSoundStorage.GetActStartSound(usedActDescription);

            var attackSoundEffectInstance = attackSoundEffect?.CreateInstance();
            return attackSoundEffectInstance;
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