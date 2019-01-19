using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Client;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на отображение модала для отображения контента контейнера.
    /// </summary>
    public class ShowInventoryModalCommand : ShowModalCommandBase
    {
        private readonly IPlayerState _playerState;

        [PublicAPI]
        [ExcludeFromCodeCoverage]
        public ShowInventoryModalCommand(ISectorModalManager modalManager, IPlayerState playerState) :
            base(modalManager)
        {
            _playerState = playerState;
        }
        
        public override void Execute()
        {
            ModalManager.ShowInventoryModal(_playerState.ActiveActor.Actor);
        }

        public override bool CanExecute()
        {
            return true;
        }
    }
}