using System;
using System.Collections.Generic;
using System.Linq;

using CDT.LIV.MonoGameClient.Scenes;

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
        private const int UNIT_SIZE = 32;

        private readonly Camera _camera;
        private readonly SpriteBatch _spriteBatch;
        private readonly IPlayer _player;
        private readonly ISectorUiState _uiState;
        private readonly Zilon.Core.Tactics.ISector _sector;

        private readonly MapViewModel _mapViewModel;
        private readonly List<GameObjectBase> _gameObjects;

        public SectorViewModel(Game game, Scenes.Camera _camera, SpriteBatch spriteBatch) : base(game)
        {
            this._camera = _camera;
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            _player = serviceScope.GetRequiredService<IPlayer>();
            _uiState = serviceScope.GetRequiredService<ISectorUiState>();
            var sector = GetPlayerSectorNode(_player).Sector;

            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            _sector = sector;

            var playerActor = (from actor in _sector.ActorManager.Items
                               where actor.Person == _player.MainPerson
                               select actor).SingleOrDefault();

            _mapViewModel = new MapViewModel(game, _uiState, _sector, spriteBatch);
            
            _gameObjects = new List<GameObjectBase>();

            foreach (var actor in _sector.ActorManager.Items)
            {
                var actorViewModel = new ActorViewModel(game, actor, _spriteBatch);

                if (actor.Person == _player.MainPerson)
                {
                    _uiState.ActiveActor = actorViewModel;
                }

                _gameObjects.Add(actorViewModel);
            }

            foreach (var staticObject in _sector.StaticObjectManager.Items)
            {
                var staticObjectModel = new StaticObjectViewModel(game, staticObject, _spriteBatch);

                _gameObjects.Add(staticObjectModel);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _mapViewModel.Draw(_camera.Transform);

            foreach (var gameObject in _gameObjects)
            {
                gameObject.Draw(gameTime, _camera.Transform);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var gameObjectsFixedList = _gameObjects.ToArray();
            foreach (var gameObject in gameObjectsFixedList)
            {
                gameObject.Update(gameTime);
            }

            var mouseState = Mouse.GetState();

            var inverseCameraTransform = Matrix.Invert(_camera.Transform);

            var mouseInWorld = Vector2.Transform(new Vector2(mouseState.X, mouseState.Y), inverseCameraTransform);

            var x = (mouseInWorld.X) / UNIT_SIZE;
            var y = (mouseInWorld.Y) / (UNIT_SIZE / 2);

            var offsetCoords = new OffsetCoords((int)x, (int)y);

            var map = _sector.Map;

            var hoverNode = map.Nodes.OfType<HexNode>()
                .SingleOrDefault(node => node.OffsetCoords == offsetCoords);

            if (hoverNode != null)
            {
                if (_uiState.HoverViewModel is null)
                {
                    _uiState.HoverViewModel = new NodeViewModel(hoverNode);
                }
                else
                {
                    if (_uiState.HoverViewModel.Item != hoverNode)
                    {
                        _uiState.HoverViewModel = new NodeViewModel(hoverNode);
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

        private static ISectorNode GetPlayerSectorNode(IPlayer player)
        {
            if (player.Globe is null)
            {
                throw new InvalidOperationException();
            }

            return (from sectorNode in player.Globe.SectorNodes
                    let sector = sectorNode.Sector
                    where sector != null
                    from actor in sector.ActorManager.Items
                    where actor.Person == player.MainPerson
                    select sectorNode).Single();
        }
    }
}
