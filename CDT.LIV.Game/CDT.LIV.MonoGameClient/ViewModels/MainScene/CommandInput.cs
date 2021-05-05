using System;
using System.Linq;

using CDT.LIV.MonoGameClient.Scenes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LIV.MonoGameClient.ViewModels.MainScene
{
    public sealed class CommandInput
    {
        private const int UNIT_SIZE = 32;

        private readonly ISectorUiState _uiState;
        private readonly ICommandPool _commandPool;
        private readonly Camera _camera;
        private readonly ISector _sector;
        private readonly ServiceProviderCommandFactory _commandFactory;

        private bool _leftMousePressed;

        public CommandInput(
            ISectorUiState sectorUiState,
            ICommandPool commandPool,
            Camera camera,
            ISector sector,
            ServiceProviderCommandFactory commandFactory)
        {
            _uiState = sectorUiState;
            _commandPool = commandPool;
            _camera = camera;
            _sector = sector;
            _commandFactory = commandFactory;
        }

        public void Update()
        {
            if (_uiState.CanPlayerGivesCommand)
            {
                var wasHotKey = HandleHotKeys();
                if (wasHotKey)
                {
                    return;
                }

                var mouseState = Mouse.GetState();

                var inverseCameraTransform = Matrix.Invert(_camera.Transform);

                var mouseInWorld = Vector2.Transform(new Vector2(mouseState.X, mouseState.Y), inverseCameraTransform);

                var offsetMouseInWorld = HexHelper.ConvertWorldToOffset((int)mouseInWorld.X, (int)mouseInWorld.Y * 2, UNIT_SIZE / 2);

                var map = _sector.Map;

                var hoverNodes = map.Nodes.OfType<HexNode>().Where(node => node.OffsetCoords == offsetMouseInWorld);
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

                    var command = _commandFactory.GetCommand<MoveCommand>();
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

        private bool HandleHotKeys()
        {
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.T))
            {
                var transitionCommand = _commandFactory.GetCommand<SectorTransitionMoveCommand>();

                _commandPool.Push(transitionCommand);

                return true;
            }

            return false;
        }
    }
}
