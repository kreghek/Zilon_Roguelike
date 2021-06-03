using System;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal class ActorViewModel : GameObjectBase, IActorViewModel
    {
        private readonly Game _game;
        private readonly IActorGraphics _graphicsRoot;
        private readonly SoundEffect _hunterDeathEffect;

        private readonly SpriteContainer _rootSprite;
        private readonly SectorViewModelContext _sectorViewModelContext;
        private readonly Sprite _shadowSprite;
        private readonly SpriteBatch _spriteBatch;
        private readonly SoundEffect _swordHitEffect;
        private readonly SoundEffect _swordMissEffect;

        private IActorStateEngine _actorStateEngine;

        public ActorViewModel(Game game,
            IActor actor,
            SectorViewModelContext sectorViewModelContext,
            IPersonVisualizationContentStorage personVisualizationContentStorage,
            SpriteBatch spriteBatch)
        {
            _game = game;
            Actor = actor;
            _sectorViewModelContext = sectorViewModelContext;
            _spriteBatch = spriteBatch;

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
                var graphicsRoot = new HumanoidGraphics(Actor.Person.GetModule<IEquipmentModule>(),
                    personVisualizationContentStorage);

                _rootSprite.AddChild(graphicsRoot);

                _graphicsRoot = graphicsRoot;
            }
            else
            {
                var graphicsRoot = new AnimalGraphics(game.Content);

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

            _actorStateEngine = new ActorIdleEngine(_graphicsRoot.RootSprite);

            _swordHitEffect = game.Content.Load<SoundEffect>("Audio/SwordHitEffect");
            //_swordMissEffect = game.Content.Load<SoundEffect>("Audio/SwordMissEffect");
            _hunterDeathEffect = game.Content.Load<SoundEffect>("Audio/HunterDeath");
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
        }

        private void Actor_DamageTaken(object? sender, DamageTakenEventArgs e)
        {
            if (sender is Actor actor)
            {
                if (actor.Person is MonsterPerson monster && monster.CheckIsDead())
                {
                    _hunterDeathEffect.CreateInstance().Play();
                }
            }
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

                var moveEngine = new ActorMoveEngine(
                    _rootSprite,
                    _graphicsRoot.RootSprite,
                    _shadowSprite,
                    newPosition,
                    animationBlockerService);
                _actorStateEngine = moveEngine;
            }
            else
            {
                _rootSprite.Position = newPosition;
            }
        }

        private void Actor_UsedAct(object? sender, UsedActEventArgs e)
        {
            var stats = e.TacticalAct.Stats;
            if (stats is null)
            {
                throw new InvalidOperationException("The act has no stats to select visualization.");
            }

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
                    _actorStateEngine =
                        new ActorMeleeAttackEngine(
                            _rootSprite,
                            targetSpritePosition,
                            animationBlockerService,
                            _swordHitEffect.CreateInstance());

                    var targetGameObject =
                        _sectorViewModelContext.GameObjects.SingleOrDefault(x => x.Node == e.TargetNode);
                    if (targetGameObject is null)
                    {
                        // This means the attacker is miss.
                        // This situation can be then the target actor moved before the attack reaches the target.
                    }
                    else
                    {
                        _sectorViewModelContext.EffectManager.HitEffects.Add(new HitEffect((LivGame)_game,
                            targetSpritePosition + targetGameObject.HitEffectPosition,
                            targetSpritePosition - _rootSprite.Position));
                    }
                }
            }
        }

        public IActor Actor { get; set; }
        public object Item => Actor;
    }
}