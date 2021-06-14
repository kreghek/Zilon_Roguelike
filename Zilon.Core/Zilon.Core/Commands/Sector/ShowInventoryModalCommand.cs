using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Client.Windows;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на отображение модала для отображения контента контейнера.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ShowInventoryModalCommand : ShowModalCommandBase
    {
        private readonly ISectorUiState _playerState;

        public ShowInventoryModalCommand(ISectorModalManager modalManager, ISectorUiState playerState) :
            base(modalManager)
        {
            _playerState = playerState;
        }

        public override CanExecuteCheckResult CanExecute()
        {
            return new CanExecuteCheckResult { IsSuccess = true };
        }

        public override void Execute()
        {
            var actor = _playerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            ModalManager.ShowInventoryModal(actor);
        }
    }
}