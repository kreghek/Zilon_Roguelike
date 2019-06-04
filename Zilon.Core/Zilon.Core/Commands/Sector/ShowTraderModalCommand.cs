using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Client;
using Zilon.Core.Persons;

namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Команда на отображение модального окна для отображения контента контейнера.
    /// </summary>
    [PublicAPI]
    public class ShowTraderModalCommand : ShowModalCommandBase
    {
        private readonly ISectorUiState _playerState;

        /// <summary>
        /// Создаёт экземпляр команды <see cref="ShowTraderModalCommand"/>.
        /// </summary>
        /// <param name="modalManager"> Менеджер модальных окон. Реализация на клиенте. </param>
        /// <param name="playerState"> Текущее состояние игрока. </param>
        [ExcludeFromCodeCoverage]
        public ShowTraderModalCommand(ISectorModalManager modalManager,
            ISectorUiState playerState) :
            base(modalManager)
        {
            _playerState = playerState;
        }

        /// <summary>
        /// Выполнение команды.
        /// </summary>
        public override void Execute()
        {
            var targetTraderViewModel = (IActorViewModel)_playerState.HoverViewModel;
            var trader = targetTraderViewModel.Actor.Person as CitizenPerson;

            ModalManager.ShowTraderModal(trader);
        }

        /// <summary>
        /// Проверяет, возможно ли выполнение команды.
        /// </summary>
        /// <returns>
        /// Возвращает true, если команду можно выполнить. Иначе возвращает false.
        /// </returns>
        public override bool CanExecute()
        {
            var targetTraderViewModel = _playerState.HoverViewModel as IActorViewModel;
            var trader = targetTraderViewModel?.Actor.Person as CitizenPerson;

            return trader != null;
        }
    }
}