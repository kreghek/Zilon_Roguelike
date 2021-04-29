using System;
using System.Linq;

using CDT.LIV.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Common;
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

        private double _idleAnimationCounter;

        private Container _rootSprite;

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
                Origin = new Vector2(0, 1)
            });

            _rootSprite.AddChild(new Sprite(_personHeadSprite)
            {
                Position = new Vector2(12, -32),
                Origin = new Vector2(0, 1)
            });
        }

        public IActor Actor { get; set; }
        public object Item => Actor;

        public override void Draw(GameTime gameTime, Matrix transform)
        {
            //_idleAnimationCounter += gameTime.ElapsedGameTime.TotalSeconds;

            _rootSprite.Rotation = (float)Math.Sin(_idleAnimationCounter);

            _spriteBatch.Begin(transformMatrix: transform);

            _rootSprite.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);

            _rootSprite.Position = new Vector2(
                (int)(playerActorWorldCoords[0] * UNIT_SIZE),
                (int)(playerActorWorldCoords[1] * UNIT_SIZE / 2)
                );
        }
    }
}
