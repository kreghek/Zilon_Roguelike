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
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    internal class ActorViewModel : GameObjectBase, IActorViewModel
    {
        private const int UNIT_SIZE = 50;

        private readonly Game _game;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _personHeadSprite;
        private readonly Texture2D _personBodySprite;

        private double _idleAnimationCounter;

        private readonly Container _rootSprite;

        private ActorMoveEngine? _actorMoveEngine;

        public ActorViewModel(Game game, IActor actor, SpriteBatch spriteBatch)
        {
            _game = game;
            Actor = actor;
            _spriteBatch = spriteBatch;

            var equipmentModule = Actor.Person.GetModuleSafe<IEquipmentModule>();

            if (equipmentModule is null)
            {
                _personHeadSprite = _game.Content.Load<Texture2D>("Sprites/Head");
                _personBodySprite = _game.Content.Load<Texture2D>("Sprites/Body");
            }
            else
            {
                for (var i = 0; i < equipmentModule.Count(); i++)
                {
                    var equipment = equipmentModule[i];
                    var slot = equipmentModule.Slots[i];

                    if (slot.Types.HasFlag(Zilon.Core.Components.EquipmentSlotTypes.Head))
                    {
                        if (equipment is null)
                        {
                            _personHeadSprite = _game.Content.Load<Texture2D>("Sprites/Head");
                        }
                        else
                        {
                            _personHeadSprite = _game.Content.Load<Texture2D>($"Sprites/equipments/{equipment.Scheme.Sid}");
                        }
                    }

                    if (slot.Types.HasFlag(Zilon.Core.Components.EquipmentSlotTypes.Body))
                    {
                        if (equipment is null)
                        {
                            _personBodySprite = _game.Content.Load<Texture2D>("Sprites/Body");
                        }
                        else
                        {
                            _personBodySprite = _game.Content.Load<Texture2D>($"Sprites/equipments/{equipment.Scheme.Sid}");
                        }
                    }
                }

                if (_personHeadSprite is null)
                {
                    _personHeadSprite = _game.Content.Load<Texture2D>("Sprites/Head");
                }

                if (_personBodySprite is null)
                {
                    _personBodySprite = _game.Content.Load<Texture2D>("Sprites/Body");
                }
            }

            _rootSprite = new Container();
            _rootSprite.AddChild(new Sprite(_personBodySprite)
            {
                Position = new Vector2(10, -10),
                ScaleScalar = 0.25f,
            });

            _rootSprite.AddChild(new Sprite(_personHeadSprite)
            {
                Position = new Vector2(10, -0),
                ScaleScalar = 0.25f
            });

            Actor.Moved += Actor_Moved;
        }

        private void Actor_Moved(object? sender, EventArgs e)
        {
            var hexSize = UNIT_SIZE / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * System.Math.Sqrt(3)),
                (float)(playerActorWorldCoords[1] * hexSize * 2 / 2)
                );

            var serviceScope = ((LivGame)_game).ServiceProvider;

            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

            var moveEngine = new ActorMoveEngine(_rootSprite, newPosition, animationBlockerService);
            _actorMoveEngine = moveEngine;
        }

        public IActor Actor { get; set; }
        public object Item => Actor;

        public override bool HiddenByFow => true;
        public override IGraphNode Node => Actor.Node;

        public override void Draw(GameTime gameTime, Matrix transform)
        {
            _idleAnimationCounter += gameTime.ElapsedGameTime.TotalSeconds;

            _rootSprite.Rotation = (float)Math.Sin(_idleAnimationCounter);

            _spriteBatch.Begin(transformMatrix: transform);

            _rootSprite.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (_actorMoveEngine != null)
            {
                _actorMoveEngine.Update(gameTime);
                if (_actorMoveEngine.IsComplete)
                {
                    _actorMoveEngine = null;
                }
            }
            else
            {
                var hexSize = UNIT_SIZE / 2;
                var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
                var newPosition = new Vector2(
                    (float)(playerActorWorldCoords[0] * hexSize * System.Math.Sqrt(3)),
                    (float)(playerActorWorldCoords[1] * hexSize * 2 / 2)
                    );

                _rootSprite.Position = newPosition;
            }
        }
    }
}
