using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics;

//TODO Сделать отдельные крипты для каждой кнопки, которые будут содежать обработчики.
//Тогда этот объект станет не нужным.
/// <summary>
/// Скрипт для обработки UI на глобальной карте.
/// </summary>
public class SectorUiHandler : MonoBehaviour
{
    [NotNull] [Inject] private readonly ISectorManager _sectorManager;

    [NotNull] [Inject] private readonly ISectorUiState _playerState;

    [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

    [NotNull] [Inject(Id = "next-turn-command")] private readonly ICommand _nextTurnCommand;

    [NotNull] [Inject(Id = "show-inventory-command")] private readonly ICommand _showInventoryCommand;

    [NotNull] [Inject(Id = "show-perks-command")] private readonly ICommand _showPersonModalCommand;

    [NotNull] [Inject(Id = "quit-request-command")] private readonly ICommand _quitRequestCommand;

    public Button NextTurnButton;
    public Button InventoryButton;
    public Button PersonButton;

    public void FixedUpdate()
    {
        if (NextTurnButton != null)
        {
            NextTurnButton.interactable = _nextTurnCommand.CanExecute();
        }

        if (InventoryButton != null)
        {
            InventoryButton.interactable = _showInventoryCommand.CanExecute();
        }

        if (PersonButton != null)
        {
            PersonButton.interactable = _showPersonModalCommand.CanExecute();
        }
    }

    public void NextTurn()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        _clientCommandExecutor.Push(_nextTurnCommand);
    }

    public void ShowInventoryButton_Handler()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        _clientCommandExecutor.Push(_showInventoryCommand);
    }

    public void ShowPersonModalButton_Handler()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        _clientCommandExecutor.Push(_showPersonModalCommand);
    }

    public void ExitGame_Handler()
    {
        _clientCommandExecutor.Push(_quitRequestCommand);
    }
}
