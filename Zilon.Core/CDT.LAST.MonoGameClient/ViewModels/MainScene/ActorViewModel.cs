using System;
using System.Linq;

using CDT.LIV.MonoGameClient.Engine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    internal class ActorViewModel : GameObjectBase, IActorViewModel
    {
        private const int UNIT_SIZE = 32;

        private readonly Game _game;
        private readonly SpriteContainer _graphicsRoot;

        private readonly SpriteContainer _rootSprite;
        private readonly SectorViewModelContext _sectorViewModelContext;
        private readonly Sprite _shadowSprite;
        private readonly SpriteBatch _spriteBatch;

        private IActorStateEngine _actorStateEngine;

        public ActorViewModel(Game game, IActor actor, SectorViewModelContext sectorViewModelContext,
            SpriteBatch spriteBatch)
        {
            _game = game;
            Actor = actor;
            _sectorViewModelContext = sectorViewModelContext;
            _spriteBatch = spriteBatch;

            var equipmentModule = Actor.Person.GetModuleSafe<IEquipmentModule>();

            var personHeadSprite = _game.Content.Load<Texture2D>("Sprites/head");
            var personBodySprite = _game.Content.Load<Texture2D>("Sprites/body");
            var personLegsSprite = _game.Content.Load<Texture2D>("Sprites/legs_idle");
            var personArmLeftSprite = _game.Content.Load<Texture2D>("Sprites/arm-left-simple");
            var personArmRightSprite = _game.Content.Load<Texture2D>("Sprites/arm-right-simple");
            var shadowTexture = _game.Content.Load<Texture2D>("Sprites/game-objects/simple-object-shadow");

            _rootSprite = new SpriteContainer();
            _shadowSprite = new Sprite(shadowTexture)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.5f),
                Color = new Color(Color.White, 0.5f)
            };

            _rootSprite.AddChild(_shadowSprite);

            var graphicsRoot = new SpriteContainer();

            _rootSprite.AddChild(graphicsRoot);

            graphicsRoot.AddChild(new Sprite(personArmLeftSprite)
            {
                Position = new Vector2(-10, -20),
                Origin = new Vector2(0.5f, 0.5f)
            });

            graphicsRoot.AddChild(new Sprite(personLegsSprite)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 0.75f)
            });

            graphicsRoot.AddChild(new Sprite(personBodySprite)
            {
                Position = new Vector2(3, -22),
                Origin = new Vector2(0.5f, 0.5f)
            });

            graphicsRoot.AddChild(new Sprite(personHeadSprite)
            {
                Position = new Vector2(-0, -20),
                Origin = new Vector2(0.5f, 1)
            });

            graphicsRoot.AddChild(new Sprite(personArmRightSprite)
            {
                Position = new Vector2(13, -20),
                Origin = new Vector2(0.5f, 0.5f)
            });

            _graphicsRoot = graphicsRoot;

            var hexSize = UNIT_SIZE / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                (float)(playerActorWorldCoords[1] * hexSize * 2 / 2)
            );

            _rootSprite.Position = newPosition;

            Actor.Moved += Actor_Moved;
            Actor.UsedAct += Actor_UsedAct;

            _actorStateEngine = new ActorIdleEngine(_graphicsRoot);
        }

        public override bool HiddenByFow => true;

        public override Vector2 HitEffectPosition => Vector2.UnitY * -24;
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
                    _actorStateEngine = new ActorIdleEngine(_graphicsRoot);

                    var hexSize = UNIT_SIZE / 2;
                    var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
                    var newPosition = new Vector2(
                        (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                        (float)(playerActorWorldCoords[1] * hexSize * 2 / 2)
                    );

                    _rootSprite.Position = newPosition;
                }
            }
        }

        private void Actor_Moved(object? sender, EventArgs e)
        {
            var hexSize = UNIT_SIZE / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                (float)(playerActorWorldCoords[1] * hexSize * 2 / 2)
            );

            if (Visible)
            {
                var serviceScope = ((LivGame)_game).ServiceProvider;

                var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

                var moveEngine = new ActorMoveEngine(_rootSprite, _graphicsRoot, _shadowSprite, newPosition,
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

            if (Visible)
            {
                if (stats.Effect == TacticalActEffectType.Damage && stats.IsMelee)
                {
                    var serviceScope = ((LivGame)_game).ServiceProvider;

                    var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

                    var hexSize = UNIT_SIZE / 2;
                    var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)e.TargetNode).OffsetCoords);
                    var newPosition = new Vector2(
                        (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                        (float)(playerActorWorldCoords[1] * hexSize * 2 / 2)
                    );

                    var targetSpritePosition = newPosition;
                    _actorStateEngine =
                        new ActorMeleeAttackEngine(_rootSprite, targetSpritePosition, animationBlockerService);

                    var targetGameObject = _sectorViewModelContext.GameObjects.Single(x => x.Node == e.TargetNode);
                    _sectorViewModelContext.EffectManager.HitEffects.Add(new HitEffect((LivGame)_game,
                        targetSpritePosition + targetGameObject.HitEffectPosition,
                        targetSpritePosition - _rootSprite.Position));
                }
            }
        }

        public IActor Actor { get; set; }
        public object Item => Actor;
    }
}