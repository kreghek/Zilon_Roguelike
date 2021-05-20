using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Client;
using Zilon.Core.Client.Windows;

namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Команда на отображение модала для отображения контента контейнера.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ShowPerksModalCommand : ShowModalCommandBase
    {
        private readonly ISectorUiState _playerState;

        [PublicAPI]
        public ShowPerksModalCommand(ISectorModalManager sectorManager, ISectorUiState playerState) :
            base(sectorManager)
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

            ModalManager.ShowPerksModal(actor);
        }
    }
}