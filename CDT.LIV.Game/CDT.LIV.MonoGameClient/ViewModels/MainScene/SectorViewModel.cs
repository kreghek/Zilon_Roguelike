using System;
using System.Collections.Generic;
using System.Linq;

using CDT.LIV.MonoGameClient.Engine;
using CDT.LIV.MonoGameClient.Scenes;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public class SectorViewModel : DrawableGameComponent
    {
        private const int UNIT_SIZE = 50;

        private readonly Camera _camera;
        private readonly SpriteBatch _spriteBatch;
        private readonly IPlayer _player;
        private readonly ISectorUiState _uiState;
        private readonly ICommandPool _commandPool;
        private readonly Zilon.Core.Tactics.ISector _sector;

        private readonly MapViewModel _mapViewModel;
        private readonly List<GameObjectBase> _gameObjects;

        private readonly Texture2D _cursorTexture;
        private readonly Texture2D _cursorTexture2;
        private readonly SpriteFont _font;
        private bool _leftMousePressed;

        public SectorViewModel(Game game, Camera _camera, SpriteBatch spriteBatch) : base(game)
        {
            this._camera = _camera;
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            _player = serviceScope.GetRequiredService<IPlayer>();
            _uiState = serviceScope.GetRequiredService<ISectorUiState>();
            _commandPool = serviceScope.GetRequiredService<ICommandPool>();

            var sector = GetPlayerSectorNode(_player).Sector;

            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            _sector = sector;

            var playerActor = (from actor in _sector.ActorManager.Items
                               where actor.Person == _player.MainPerson
                               select actor).SingleOrDefault();

            _mapViewModel = new MapViewModel(game, _player, _uiState, _sector, spriteBatch);
            
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

            _cursorTexture = Game.Content.Load<Texture2D>("Sprites/ui/walk-cursor");
            _cursorTexture2 = Game.Content.Load<Texture2D>("Sprites/hex");
            _font = Game.Content.Load<SpriteFont>("Fonts/Main");
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _mapViewModel.Draw(/*_camera.Transform*/Matrix.Identity);

            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            var fowData = _player.MainPerson.GetModule<IFowData>();
            var visibleFowNodeData = fowData.GetSectorFowData(_sector);

            if (visibleFowNodeData is null)
            {
                throw new InvalidOperationException();
            }

            foreach (var gameObject in _gameObjects)
            {
                var fowNode = visibleFowNodeData.Nodes.SingleOrDefault(x=>x.Node == gameObject.Node);

                if (fowNode is null)
                {
                    continue;
                }

                if (fowNode.State != Zilon.Core.Tactics.SectorMapNodeFowState.Observing && gameObject.HiddenByFow)
                {
                    continue;
                }

                gameObject.Draw(gameTime, Matrix.Identity);
            }





            var mouseState = Mouse.GetState();
            _spriteBatch.Begin(transformMatrix: Matrix.Identity);

            var inverseCameraTransform = Matrix.Invert(Matrix.Identity);

            var mouseInWorld = Vector2.Transform(new Vector2(mouseState.X, mouseState.Y), inverseCameraTransform);

            _spriteBatch.Draw(_cursorTexture, new Vector2(mouseInWorld.X, mouseInWorld.Y), Color.White);

            var offsetMouseInWorld = HexHelper.ConvertWorldToOffset((int)mouseState.X, (int)mouseState.Y * 2, UNIT_SIZE / 2);
            var worldOffsetMouse = HexHelper.ConvertToWorld(offsetMouseInWorld);

            var hexSize = UNIT_SIZE / 2;
            var sprite = new Sprite(_cursorTexture2,
                size: new Point(
                    (int)(hexSize * System.Math.Sqrt(3)),
                    hexSize * 2 / 2
                    ),
                color: Color.Black);

            sprite.Position = new Vector2(
                (float)(worldOffsetMouse[0] * hexSize * System.Math.Sqrt(3)),
                (float)(worldOffsetMouse[1] * hexSize * 2 / 2)
                );

            //sprite.Draw(_spriteBatch);

            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, $"{mouseState.X}:{mouseState.Y}", new Vector2(0, 0), Color.White);
            _spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var gameObjectsFixedList = _gameObjects.ToArray();
            foreach (var gameObject in gameObjectsFixedList)
            {
                gameObject.Update(gameTime);
            }

            if (_uiState.CanPlayerGivesCommand)
            {
                var mouseState = Mouse.GetState();

                var inverseCameraTransform = Matrix.Invert(_camera.Transform);

                var mouseInWorld = Vector2.Transform(new Vector2(mouseState.X, mouseState.Y), inverseCameraTransform);

                var offsetMouseInWorld = HexHelper.ConvertWorldToOffset((int)mouseState.X, (int)mouseState.Y * 2, UNIT_SIZE / 2);

                var map = _sector.Map;

                var hoverNodes = map.Nodes.OfType<HexNode>()
                    .Where(node => node.OffsetCoords == offsetMouseInWorld);
                var hoverNode = hoverNodes.FirstOrDefault();

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

                if (!_leftMousePressed 
                    && mouseState.LeftButton == ButtonState.Pressed
                    && _uiState.HoverViewModel != null 
                    && _uiState.CanPlayerGivesCommand)
                {
                    _leftMousePressed = true;

                    _uiState.SelectedViewModel = _uiState.HoverViewModel;

                    var serviceScope = ((LivGame)Game).ServiceProvider;

                    var command = serviceScope.GetRequiredService<MoveCommand>();
                    if (command.CanExecute().IsSuccess)
                    {
                        _commandPool.Push(command);
                    }
                }

                if (_leftMousePressed && mouseState.LeftButton == ButtonState.Released)
                {
                    _leftMousePressed = false;
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
