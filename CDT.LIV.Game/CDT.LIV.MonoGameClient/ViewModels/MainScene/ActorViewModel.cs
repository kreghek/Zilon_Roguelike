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
        private const int UNIT_SIZE = 32;

        private readonly Game _game;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _personHeadSprite;
        private readonly Texture2D _personBodySprite;

        private readonly Container _rootSprite;

        private IActorStateEngine _actorStateEngine;

        public ActorViewModel(Game game, IActor actor, SpriteBatch spriteBatch)
        {
            _game = game;
            Actor = actor;
            _spriteBatch = spriteBatch;

            var equipmentModule = Actor.Person.GetModuleSafe<IEquipmentModule>();

            _personHeadSprite = _game.Content.Load<Texture2D>("Sprites/head");
            _personBodySprite = _game.Content.Load<Texture2D>("Sprites/body");

            //if (equipmentModule is null)
            //{
            //    _personHeadSprite = _game.Content.Load<Texture2D>("Sprites/head");
            //    _personBodySprite = _game.Content.Load<Texture2D>("Sprites/body");
            //}
            //else
            //{
            //    for (var i = 0; i < equipmentModule.Count(); i++)
            //    {
            //        var equipment = equipmentModule[i];
            //        var slot = equipmentModule.Slots[i];

            //        if (slot.Types.HasFlag(Zilon.Core.Components.EquipmentSlotTypes.Head))
            //        {
            //            if (equipment is null)
            //            {
            //                _personHeadSprite = _game.Content.Load<Texture2D>("Sprites/head");
            //            }
            //            else
            //            {
            //                _personHeadSprite = _game.Content.Load<Texture2D>($"Sprites/equipments/{equipment.Scheme.Sid}");
            //            }
            //        }

            //        if (slot.Types.HasFlag(Zilon.Core.Components.EquipmentSlotTypes.Body))
            //        {
            //            if (equipment is null)
            //            {
            //                _personBodySprite = _game.Content.Load<Texture2D>("Sprites/body");
            //            }
            //            else
            //            {
            //                _personBodySprite = _game.Content.Load<Texture2D>($"Sprites/equipments/{equipment.Scheme.Sid}");
            //            }
            //        }
            //    }

            //if (_personHeadSprite is null)
            //    {
            //        _personHeadSprite = _game.Content.Load<Texture2D>("Sprites/head");
            //    }

            //    if (_personBodySprite is null)
            //    {
            //        _personBodySprite = _game.Content.Load<Texture2D>("Sprites/body");
            //    }
            //}

            _rootSprite = new Container();
            _rootSprite.AddChild(new Sprite(_personBodySprite)
            {
                Position = new Vector2(0, 0),
                Origin = new Vector2(0.5f, 1)
            });

            _rootSprite.AddChild(new Sprite(_personHeadSprite)
            {
                Position = new Vector2(-6, -32),
                Origin = new Vector2(0.5f, 1)
            });

            var hexSize = UNIT_SIZE / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                (float)(playerActorWorldCoords[1] * hexSize * 2 / 2)
                );

            _rootSprite.Position = newPosition;

            Actor.Moved += Actor_Moved;

            _actorStateEngine = new ActorIdleEngine(_rootSprite);
        }

        private void Actor_Moved(object? sender, EventArgs e)
        {
            var hexSize = UNIT_SIZE / 2;
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);
            var newPosition = new Vector2(
                (float)(playerActorWorldCoords[0] * hexSize * Math.Sqrt(3)),
                (float)(playerActorWorldCoords[1] * hexSize * 2 / 2)
                );

            var serviceScope = ((LivGame)_game).ServiceProvider;

            var animationBlockerService = serviceScope.GetRequiredService<IAnimationBlockerService>();

            var moveEngine = new ActorMoveEngine(_rootSprite, newPosition, animationBlockerService);
            _actorStateEngine = moveEngine;
        }

        public IActor Actor { get; set; }
        public object Item => Actor;

        public override bool HiddenByFow => true;
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
                    _actorStateEngine = new ActorIdleEngine(_rootSprite);

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
    }
}
