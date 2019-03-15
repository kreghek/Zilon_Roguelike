using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Команда на отображение модала для отображения контента контейнера.
    /// </summary>
    public class ShowPerksModalCommand : ShowModalCommandBase
    {
        private readonly ISectorUiState _playerState;

        [PublicAPI]
        [ExcludeFromCodeCoverage]
        public ShowPerksModalCommand(ISectorModalManager sectorManager, ISectorUiState playerState) :
            base(sectorManager)
        {
            _playerState = playerState;
        }
        
        public override void Execute()
        {
            ModalManager.ShowPerksModal(_playerState.ActiveActor.Actor);
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}