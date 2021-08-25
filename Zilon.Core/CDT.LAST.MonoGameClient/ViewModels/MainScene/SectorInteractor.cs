﻿using System;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Screens;
using CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Zilon.Core;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal sealed class SectorInteractor
    {
        private readonly Camera _camera;
        private readonly ServiceProviderCommandFactory _commandFactory;
        private readonly ICommandPool _commandPool;
        private readonly ISector _sector;
        private readonly SectorViewModelContext _sectorViewModelContext;
        private readonly ISectorUiState _uiState;

        private KeyboardState? _lastKeyboardState;

        private bool _leftMousePressed;

        public SectorInteractor(
            ISectorUiState sectorUiState,
            ICommandPool commandPool,
            Camera camera,
            ISector sector,
            SectorViewModelContext sectorViewModelContext,
            ServiceProviderCommandFactory commandFactory)
        {
            _uiState = sectorUiState;
            _commandPool = commandPool;
            _camera = camera;
            _sector = sector;
            _sectorViewModelContext = sectorViewModelContext;
            _commandFactory = commandFactory;
        }

        public void Update(SectorViewModelContext sectorViewModelContext)
        {
            if (!_uiState.CanPlayerGivesCommand)
            {
                return;
            }

            var wasHotKey = HandleHotKeys();
            if (wasHotKey)
            {
                return;
            }

            ProcessMouseCommands(sectorViewModelContext);

            ProcessKeyboardCommands(sectorViewModelContext);
        }

        private bool DidOpenDropMenu(KeyboardState keyboardState)
        {
            if (!keyboardState.IsKeyUp(Keys.O) || _lastKeyboardState?.IsKeyDown(Keys.O) != true)
            {
                return false;
            }

            _lastKeyboardState = keyboardState;

            var openCommand = _commandFactory.GetCommand<OpenContainerCommand>();

            var actor = _uiState.ActiveActor?.Actor;
            if (actor is null)
            {
                Debug.Fail(
                    "Active actor must be assigned before the sector view model and the sector interactor start processing of user input.");
                return true;
            }

            _uiState.SelectedViewModel = GetStaticObjectUnderActor(actor);
            if (openCommand.CanExecute().IsSuccess)
            {
                _commandPool.Push(openCommand);
            }

            return true;
        }

        private bool DidTransferToOtherSector(KeyboardState keyboardState)
        {
            if (!keyboardState.IsKeyUp(Keys.T) || _lastKeyboardState?.IsKeyDown(Keys.T) != true)
            {
                return false;
            }

            _lastKeyboardState = keyboardState;

            var transitionCommand = _commandFactory.GetCommand<SectorTransitionMoveCommand>();

            if (transitionCommand.CanExecute().IsSuccess)
            {
                _commandPool.Push(transitionCommand);
            }

            return true;
        }

        private ISelectableViewModel? GetStaticObjectUnderActor(IActor actor)
        {
            var staticObjectsUnderActor = _sector.StaticObjectManager.Items.Where(x => x.Node == actor.Node).ToArray();
            Debug.Assert(staticObjectsUnderActor.Length < 2,
                "There is no way to put multiple passable objects in same node.");

            var staticObjectUnderActor = staticObjectsUnderActor.FirstOrDefault();

            if (staticObjectUnderActor is null)
            {
                return null;
            }

            return _sectorViewModelContext.GameObjects.OfType<IContainerViewModel>()
                .SingleOrDefault(x => x.StaticObject == staticObjectUnderActor);
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
            if (DidTransferToOtherSector(keyboardState))
            {
                return true;
            }

            if (DidOpenDropMenu(keyboardState))
            {
                return true;
            }

            _lastKeyboardState = keyboardState;

            return false;
        }

        private void ProcessKeyboardCommands(SectorViewModelContext sectorViewModelContext)
        {
            if (!_uiState.CanPlayerGivesCommand)
            {
                return;
            }

            var keyboardState = Keyboard.GetState();

            var map = _sector.Map;

            var actor = _uiState.ActiveActor?.Actor;

            if (actor is null)
            {
                return;
            }

            var actorNode = (HexNode)actor.Node;

            var currentCoords = actorNode.OffsetCoords;

            OffsetCoords? moveCoords = default;

            if (keyboardState.IsKeyDown(Keys.NumPad4))
            {
                moveCoords = currentCoords.Left();
            }

            if (keyboardState.IsKeyDown(Keys.NumPad1))
            {
                moveCoords = currentCoords.LeftDown();
            }

            if (keyboardState.IsKeyDown(Keys.NumPad7))
            {
                moveCoords = currentCoords.LeftUp();
            }

            if (keyboardState.IsKeyDown(Keys.NumPad6))
            {
                moveCoords = currentCoords.Right();
            }

            if (keyboardState.IsKeyDown(Keys.NumPad3))
            {
                moveCoords = currentCoords.RightDown();
            }

            if (keyboardState.IsKeyDown(Keys.NumPad9))
            {
                moveCoords = currentCoords.RightUp();
            }

            if (moveCoords == default)
            {
                return;
            }

            var moveNode = map.Nodes.OfType<HexNode>().FirstOrDefault(node => node.OffsetCoords == moveCoords);
            var nodeVm = GetViewModelByNode(sectorViewModelContext, _uiState.HoverViewModel, moveNode);

            if (nodeVm is null)
            {
                return;
            }

            _uiState.HoverViewModel = nodeVm;
            _uiState.SelectedViewModel = nodeVm;

            var command = SelectCommandBySelectedViewModel(
                nodeVm,
                _commandFactory,
                _uiState);

            if (command.CanExecute().IsSuccess)
            {
                _commandPool.Push(command);
            }
        }

        private void ProcessMouseCommands(SectorViewModelContext sectorViewModelContext)
        {
            var mouseState = Mouse.GetState();

            var inverseCameraTransform = Matrix.Invert(_camera.Transform);

            var mouseInWorld = Vector2.Transform(new Vector2(mouseState.X, mouseState.Y), inverseCameraTransform);

            var offsetMouseInWorld =
                HexHelper.ConvertWorldToOffset((int)mouseInWorld.X, (int)mouseInWorld.Y * 2,
                    (int)(MapMetrics.UnitSize / 2));

            var map = _sector.Map;

            var hoverNodes = map.Nodes.OfType<HexNode>().Where(node => node.OffsetCoords == offsetMouseInWorld);
            var hoverNode = hoverNodes.FirstOrDefault();

            _uiState.HoverViewModel =
                GetViewModelByNode(sectorViewModelContext, _uiState.HoverViewModel, hoverNode);

            if (!_leftMousePressed
                && mouseState.LeftButton == ButtonState.Pressed
                && _uiState.HoverViewModel != null
                && _uiState.CanPlayerGivesCommand
                && !BottomMenuPanel.MouseIsOver
                && !PersonMarkersPanel.MouseIsOver)
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

                    if (_uiState.TacticalAct is null)
                    {
                        SelectPunchAsDefaultCombatAct(_uiState);
                    }

                    return commandFactory.GetCommand<AttackCommand>();

                case IMapNodeViewModel:
                    return commandFactory.GetCommand<MoveCommand>();

                default:
                    throw new InvalidOperationException(
                        $"Object of unknown type (${selectedViewModel.GetType()}) was selected.");
            }
        }

        private static void SelectPunchAsDefaultCombatAct(ISectorUiState uiState)
        {
            var availableCombatActs =
                uiState.ActiveActor.Actor.Person.GetModule<ICombatActModule>().GetCurrentCombatActs();
            var punchAct = availableCombatActs.Single(x => x.Scheme.Sid == "punch");

            uiState.TacticalAct = punchAct;
        }
    }
}