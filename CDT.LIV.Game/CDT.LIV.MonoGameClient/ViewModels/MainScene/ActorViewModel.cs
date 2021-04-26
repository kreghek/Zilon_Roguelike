using System.Linq;

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
        private const int UNIT_SIZE = 50;

        private readonly Game _game;
        private readonly SpriteBatch _spriteBatch;
        private readonly Texture2D _personHeadSprite;
        private readonly Texture2D _personBodySprite;

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
            }
        }

        public IActor Actor { get; set; }
        public object Item => Actor;

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)Actor.Node).OffsetCoords);

            _spriteBatch.Draw(_personBodySprite,
               new Rectangle(
                   (int)(playerActorWorldCoords[0] * UNIT_SIZE),
                   (int)(playerActorWorldCoords[1] * UNIT_SIZE / 2 - UNIT_SIZE * 0.45f),
                   UNIT_SIZE,
                   UNIT_SIZE),
               Color.White);

            _spriteBatch.Draw(_personHeadSprite,
                new Rectangle(
                    (int)(playerActorWorldCoords[0] * UNIT_SIZE + UNIT_SIZE * 0.25f),
                    (int)(playerActorWorldCoords[1] * UNIT_SIZE / 2 - UNIT_SIZE * 0.5f),
                    (int)(UNIT_SIZE * 0.5),
                    (int)(UNIT_SIZE * 0.5)),
                Color.White);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
