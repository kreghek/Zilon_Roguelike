using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

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
            var uiState = serviceScope.GetRequiredService<ISectorUiState>();

            _spriteBatch.Begin();

            var playerActor = player.SectorNode.Sector.ActorManager.Items.Single(x=>x.Person == player.MainPerson);

            var playerActorWorldCoords = HexHelper.ConvertToWorld(((HexNode)playerActor.Node).OffsetCoords);

            var screenWidth = 800;
            var screenHeight = 600;

            foreach (HexNode node in player.SectorNode.Sector.Map.Nodes)
            {
                var worldCoords = HexHelper.ConvertToWorld(node.OffsetCoords);

                if (uiState.HoverViewModel != null && node == uiState.HoverViewModel.Item)
                {
                    _spriteBatch.Draw(hexSprite, new Rectangle((int)(worldCoords[0] * UNIT_SIZE), (int)(worldCoords[1] * UNIT_SIZE / 2), UNIT_SIZE, UNIT_SIZE / 2), Color.CornflowerBlue);
                }
                else
                {
                    _spriteBatch.Draw(hexSprite, new Rectangle((int)(worldCoords[0] * UNIT_SIZE), (int)(worldCoords[1] * UNIT_SIZE / 2), UNIT_SIZE, UNIT_SIZE / 2), Color.White);
                }
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mouseState = Mouse.GetState(Game.Window);

            var x = mouseState.X / UNIT_SIZE;
            var y = mouseState.Y / (UNIT_SIZE / 2);

            var offsetCoords = new OffsetCoords(x, y);

            var serviceScope = ((LivGame)Game).ServiceProvider;
            var uiState = serviceScope.GetRequiredService<ISectorUiState>();

            var player = serviceScope.GetRequiredService<IPlayer>();
            var globe = player.Globe;

            var playerActorSectorNode = GetPlayerSectorNode(player, globe);

            var map = playerActorSectorNode.Sector.Map;

            var hoverNode = map.Nodes.OfType<HexNode>()
                .SingleOrDefault(node => node.OffsetCoords == offsetCoords);

            if (hoverNode != null)
            {
                uiState.HoverViewModel = new NodeViewModel { Node = hoverNode };
            }
            else
            {
                uiState.HoverViewModel = null;
            }

            if (mouseState.LeftButton == ButtonState.Pressed && uiState.HoverViewModel != null)
            {
                var playerActor = (from sectorNode in globe.SectorNodes
                                   from actor in sectorNode.Sector.ActorManager.Items
                                   where actor.Person == player.MainPerson
                                   select actor).SingleOrDefault();

                uiState.ActiveActor = new ActorViewModel { Actor = playerActor };
                

                uiState.SelectedViewModel = uiState.HoverViewModel;

                var command = serviceScope.GetRequiredService<MoveCommand>();
                if (command.CanExecute())
                {
                    command.Execute();
                }
            }
        }
        private static ISectorNode GetPlayerSectorNode(IPlayer player, IGlobe globe)
        {
            return (from sectorNode in globe.SectorNodes
                    from actor in sectorNode.Sector.ActorManager.Items
                    where actor.Person == player.MainPerson
                    select sectorNode).SingleOrDefault();
        }
    }

    internal class NodeViewModel : IMapNodeViewModel
    {
        public object Item => Node;
        public HexNode Node { get; set; }
    }
}
