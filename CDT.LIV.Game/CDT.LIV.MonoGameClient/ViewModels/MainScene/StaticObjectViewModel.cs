using CDT.LIV.MonoGameClient.Engine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Graphs;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    class StaticObjectViewModel : GameObjectBase, IContainerViewModel
    {
        private const int UNIT_SIZE = 32;

        private Game _game;
        private SpriteBatch _spriteBatch;
        private readonly Container _rootSprite;
        private readonly Texture2D _personHeadSprite;

        public StaticObjectViewModel(Game game, IStaticObject staticObject, SpriteBatch spriteBatch)
        {
            _game = game;
            StaticObject = staticObject;
            _spriteBatch = spriteBatch;

            _personHeadSprite = _game.Content.Load<Texture2D>("Sprites/Head");

            var worldCoords = HexHelper.ConvertToWorld(((HexNode)StaticObject.Node).OffsetCoords);

            var hexSize = UNIT_SIZE / 2;
            var staticObjectPosition = new Vector2(
                (float)(worldCoords[0] * hexSize * System.Math.Sqrt(3)),
                (float)(worldCoords[1] * hexSize * 2 / 2)
                );

            _rootSprite = new Container();
            _rootSprite.AddChild(new Sprite(_personHeadSprite, origin: new Vector2(0.5f, 1), position: staticObjectPosition, color: Color.Black));
        }

        public IStaticObject StaticObject { get; set; }
        public object Item => StaticObject;

        public override bool HiddenByFow => false;
        public override IGraphNode Node => StaticObject.Node;

        public override void Draw(GameTime gameTime, Matrix transform)
        {
            _spriteBatch.Begin(transformMatrix: transform);

            _rootSprite.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
