using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public class SectorViewModel : DrawableGameComponent
    {
        private const int UNIT_SIZE = 50;

        private readonly SpriteBatch _spriteBatch;
        private readonly IPlayer _player;
        private readonly ISectorUiState _uiState;
        private readonly Zilon.Core.Tactics.ISector _sector;

        private readonly MapViewModel _mapViewModel;
        private readonly List<GameObjectBase> _gameObjects;

        public SectorViewModel(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            _player = serviceScope.GetRequiredService<IPlayer>();
            _uiState = serviceScope.GetRequiredService<ISectorUiState>();
            _sector = GetPlayerSectorNode(_player, _player.Globe).Sector;

            var playerActor = (from sectorNode in _player.Globe.SectorNodes
                               from actor in sectorNode.Sector.ActorManager.Items
                               where actor.Person == _player.MainPerson
                               select actor).SingleOrDefault();

            _mapViewModel = new MapViewModel(game, _uiState, _sector, spriteBatch);
            
            _gameObjects = new List<GameObjectBase>();

            foreach (var actor in _sector.ActorManager.Items)
            {
                var actorViewModel = new ActorViewModel(game, _spriteBatch);
                actorViewModel.Actor = actor;
                
                if (actor.Person == _player.MainPerson)
                {
                    _uiState.ActiveActor = actorViewModel;
                }

                _gameObjects.Add(actorViewModel);
            }

            foreach (var staticObject in _sector.StaticObjectManager.Items)
            {
                var staticObjectModel = new StaticObjectViewModel(game, _spriteBatch);
                staticObjectModel.StaticObject = staticObject;
                _gameObjects.Add(staticObjectModel);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _mapViewModel.Draw();

            foreach (var gameObject in _gameObjects)
            {
                gameObject.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mouseState = Mouse.GetState();

            var x = (mouseState.X) / UNIT_SIZE;
            var y = (mouseState.Y) / (UNIT_SIZE / 2);

            var offsetCoords = new OffsetCoords(x, y);

            var map = _sector.Map;

            var hoverNode = map.Nodes.OfType<HexNode>()
                .SingleOrDefault(node => node.OffsetCoords == offsetCoords);

            if (hoverNode != null)
            {
                if (_uiState.HoverViewModel is null)
                {
                    _uiState.HoverViewModel = new NodeViewModel { Node = hoverNode };
                }
                else
                {
                    if (_uiState.HoverViewModel.Item != hoverNode)
                    {
                        _uiState.HoverViewModel = new NodeViewModel { Node = hoverNode };
                    }
                }
            }
            else
            {
                _uiState.HoverViewModel = null;
            }

            if (mouseState.LeftButton == ButtonState.Pressed && _uiState.HoverViewModel != null)
            {
                _uiState.SelectedViewModel = _uiState.HoverViewModel;

                var serviceScope = ((LivGame)Game).ServiceProvider;

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
}
