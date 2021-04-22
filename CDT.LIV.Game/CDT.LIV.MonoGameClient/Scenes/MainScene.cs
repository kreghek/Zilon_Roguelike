using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Common;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LIV.MonoGameClient.Scenes
{
    class MainScene : GameSceneBase
    {
        private const int UNIT_SIZE = 50;
        private readonly SpriteBatch _spriteBatch;

        public MainScene(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var hexSprite = Game.Content.Load<Texture2D>("Sprites/hex");

            var serviceScope = ((LivGame)Game).ServiceProvider;

            var player = serviceScope.GetRequiredService<IPlayer>();

            _spriteBatch.Begin();
            foreach (HexNode node in player.SectorNode.Sector.Map.Nodes)
            {
                var worldCoords = HexHelper.ConvertToWorld(node.OffsetCoords);
                _spriteBatch.Draw(hexSprite, new Rectangle((int)(worldCoords[0] * UNIT_SIZE), (int)(worldCoords[1] * UNIT_SIZE / 2), UNIT_SIZE, UNIT_SIZE / 2), Color.White);
            }
            _spriteBatch.End();
        }
    }
}
