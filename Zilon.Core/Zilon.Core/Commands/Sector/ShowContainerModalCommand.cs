﻿using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Client.Windows;
using Zilon.Core.PersonModules;
using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Команда на отображение модального окна для отображения контента контейнера.
    /// </summary>
    public class ShowContainerModalCommand : ShowModalCommandBase
    {
        private readonly ISectorUiState _playerState;

        [ExcludeFromCodeCoverage]
        public ShowContainerModalCommand(ISectorModalManager modalManager, ISectorUiState playerState) :
            base(modalManager)
        {
            _playerState = playerState;
        }

        public override CanExecuteCheckResult CanExecute()
        {
            var activeActor = _playerState.ActiveActor!;

            var inventory = activeActor.Actor.Person.GetModule<IInventoryModule>();

            var targetContainerViewModel = _playerState.HoverViewModel as IContainerViewModel;
            var container = targetContainerViewModel?.StaticObject;
            var containerContent = container?.GetModule<IPropContainer>().Content;

            return new CanExecuteCheckResult { IsSuccess = inventory != null && containerContent != null };
        }

        public override void Execute()
        {
            var activeActor = _playerState.ActiveActor!;

            var inventory = activeActor.Actor.Person.GetModule<IInventoryModule>();
            var targetContainerViewModel = (IContainerViewModel?)_playerState.HoverViewModel;
            var container = targetContainerViewModel!.StaticObject;
            var containerContent = container.GetModule<IPropContainer>().Content;
            var transferMachine = new PropTransferMachine(inventory, containerContent);

            ModalManager.ShowContainerModal(transferMachine);
        }
    }
}