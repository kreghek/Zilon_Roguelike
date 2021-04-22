using System.Linq;

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
            var personHeadSprite = Game.Content.Load<Texture2D>("Sprites/Head");
            var personBodySprite = Game.Content.Load<Texture2D>("Sprites/Body");

            var serviceScope = ((LivGame)Game).ServiceProvider;

            var player = serviceScope.GetRequiredService<IPlayer>();

            _spriteBatch.Begin();

            var playerActor = player.SectorNode.Sector.ActorManager.Items.Single(x=>x.Person == player.MainPerson);

            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)playerActor.Node).OffsetCoords);

            var screenWidth = 800;
            var screenHeight = 600;

            foreach (HexNode node in player.SectorNode.Sector.Map.Nodes)
            {
                var worldCoords = HexHelper.ConvertToWorld(node.OffsetCoords);
                _spriteBatch.Draw(hexSprite, new Rectangle((int)(worldCoords[0] * UNIT_SIZE), (int)(worldCoords[1] * UNIT_SIZE / 2), UNIT_SIZE, UNIT_SIZE / 2), Color.White);
            }

            _spriteBatch.Draw(personBodySprite,
               new Rectangle(
                   (int)(playerActorWorldCoords[0] * UNIT_SIZE),
                   (int)(playerActorWorldCoords[1] * UNIT_SIZE / 2 - UNIT_SIZE * 0.45f),
                   (int)(UNIT_SIZE),
                   (int)(UNIT_SIZE)),
               Color.White);

            _spriteBatch.Draw(personHeadSprite,
                new Rectangle(
                    (int)(playerActorWorldCoords[0] * UNIT_SIZE + UNIT_SIZE * 0.25f),
                    (int)(playerActorWorldCoords[1] * UNIT_SIZE / 2 - UNIT_SIZE * 0.5f),
                    (int)(UNIT_SIZE * 0.5),
                    (int)(UNIT_SIZE * 0.5)),
                Color.White);

            _spriteBatch.End();
        }
    }
}
