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
        private const int UNIT_SIZE = 32;

        private Game _game;
        private SpriteBatch _spriteBatch;
        private readonly Texture2D _personHeadSprite;

        public StaticObjectViewModel(Game game, IStaticObject staticObject, SpriteBatch spriteBatch)
        {
            _game = game;
            StaticObject = staticObject;
            _spriteBatch = spriteBatch;

            _personHeadSprite = _game.Content.Load<Texture2D>("Sprites/Head");
        }

        public IStaticObject StaticObject { get; set; }
        public object Item => StaticObject;

        public override void Draw(GameTime gameTime, Matrix transform)
        {
            _spriteBatch.Begin(transformMatrix: transform);

            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)StaticObject.Node).OffsetCoords);

            _spriteBatch.Draw(_personHeadSprite,
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
