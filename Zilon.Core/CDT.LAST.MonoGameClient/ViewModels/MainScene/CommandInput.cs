using System;
using System.Linq;

using CDT.LAST.MonoGameClient.Scenes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    public sealed class CommandInput
    {
        private const int UNIT_SIZE = 32;
        private readonly Camera _camera;
        private readonly ServiceProviderCommandFactory _commandFactory;
        private readonly ICommandPool _commandPool;
        private readonly ISector _sector;

        private readonly ISectorUiState _uiState;

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

        public void Update(SectorViewModelContext sectorViewModelContext)
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

                var offsetMouseInWorld =
                    HexHelper.ConvertWorldToOffset((int)mouseInWorld.X, (int)mouseInWorld.Y * 2, UNIT_SIZE / 2);

                var map = _sector.Map;

                var hoverNodes = map.Nodes.OfType<HexNode>().Where(node => node.OffsetCoords == offsetMouseInWorld);
                var hoverNode = hoverNodes.FirstOrDefault();

                _uiState.HoverViewModel =
                    GetViewModelByNode(sectorViewModelContext, _uiState.HoverViewModel, hoverNode);

                if (!_leftMousePressed
                    && mouseState.LeftButton == ButtonState.Pressed
                    && _uiState.HoverViewModel != null
                    && _uiState.CanPlayerGivesCommand)
                {
                    _leftMousePressed = true;

                    _uiState.SelectedViewModel = _uiState.HoverViewModel;
                    var command = SelectCommandBySelectedViewModel(
                        _uiState.SelectedViewModel,
                        _commandFactory,
                        _uiState);

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

        private static ISelectableViewModel? GetViewModelByNode(SectorViewModelContext sectorViewModelContext,
            ISelectableViewModel? currentSelectedViewModel,
            HexNode? hoverNode)
        {
            if (hoverNode != null)
            {
                var actorsInThisNode =
                    sectorViewModelContext.GetActors().SingleOrDefault(x => ReferenceEquals(x.Node, hoverNode));
                if (actorsInThisNode is null)
                {
                    if (currentSelectedViewModel is null)
                    {
                        return new NodeViewModel(hoverNode);
                    }

                    if (currentSelectedViewModel.Item != hoverNode)
                    {
                        return new NodeViewModel(hoverNode);
                    }

                    return currentSelectedViewModel;
                }

                var actorViewModel = sectorViewModelContext.GameObjects.OfType<IActorViewModel>()
                    .SingleOrDefault(x => x.Actor == actorsInThisNode);

                return actorViewModel;
            }

            return null;
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

        private static ICommand SelectCommandBySelectedViewModel(ISelectableViewModel selectedViewModel,
            ServiceProviderCommandFactory commandFactory, ISectorUiState _uiState)
        {
            switch (selectedViewModel)
            {
                case IActorViewModel:
                    var activeActor = _uiState.ActiveActor;
                    if (activeActor is null)
                    {
                        throw new InvalidOperationException();
                    }

                    _uiState.TacticalAct = activeActor.Actor.Person.GetModule<ICombatActModule>()
                        .CalcCombatActs().First();

                    return commandFactory.GetCommand<AttackCommand>();

                case IMapNodeViewModel:
                    return commandFactory.GetCommand<MoveCommand>();

                default:
                    throw new InvalidOperationException(
                        $"Object of unknown type (${selectedViewModel.GetType()}) was selected.");
            }
        }
    }
}