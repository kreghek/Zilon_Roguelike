using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    class StaticObjectViewModel : GameObjectBase, IContainerViewModel
    {
        private const int UNIT_SIZE = 50;

        private Game _game;
        private SpriteBatch _spriteBatch;

        public StaticObjectViewModel(Game game, SpriteBatch spriteBatch)
        {
            _game = game;
            _spriteBatch = spriteBatch;
        }

        public IStaticObject StaticObject { get; set; }
        public object Item { get; }

        public override void Draw(GameTime gameTime)
        {
            var personHeadSprite = _game.Content.Load<Texture2D>("Sprites/Head");

            _spriteBatch.Begin();

            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)StaticObject.Node).OffsetCoords);

            _spriteBatch.Draw(personHeadSprite,
                new Rectangle(
                    (int)(playerActorWorldCoords[0] * UNIT_SIZE + UNIT_SIZE * 0.25f),
                    (int)(playerActorWorldCoords[1] * UNIT_SIZE / 2),
                    (int)(UNIT_SIZE * 0.5),
                    (int)(UNIT_SIZE * 0.5)),
                Color.Black);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
