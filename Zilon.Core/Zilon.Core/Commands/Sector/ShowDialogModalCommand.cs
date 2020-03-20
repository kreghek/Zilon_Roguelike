using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Client;
using Zilon.Core.Client.Windows;
using Zilon.Core.Persons;

namespace Zilon.Core.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Команда на отображение модального окна для отображения контента контейнера.
    /// </summary>
    [PublicAPI]
    public class ShowDialogModalCommand : ShowModalCommandBase
    {
        private readonly ISectorUiState _playerState;

        /// <summary>
        /// Создаёт экземпляр команды <see cref="ShowTraderModalCommand"/>.
        /// </summary>
        /// <param name="modalManager"> Менеджер модальных окон. Реализация на клиенте. </param>
        /// <param name="playerState"> Текущее состояние игрока. </param>
        [ExcludeFromCodeCoverage]
        public ShowDialogModalCommand(ISectorModalManager modalManager,
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
            var citizen = (CitizenPerson)targetTraderViewModel.Actor.Person;

            ModalManager.ShowDialogModal(citizen);
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
            var citizen = targetTraderViewModel?.Actor.Person as CitizenPerson;

            return citizen != null;
        }
    }
}