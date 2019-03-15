using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics;

public class UiHandler : MonoBehaviour
{
    [NotNull] [Inject] private readonly ISectorManager _sectorManager;

    [NotNull] [Inject] private readonly ISectorUiState _playerState;

    [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

    [NotNull] [Inject(Id = "next-turn-command")] private readonly ICommand _nextTurnCommand;

    [NotNull] [Inject(Id = "show-inventory-command")] private readonly ICommand _showInventoryCommand;

    [NotNull] [Inject(Id = "show-perks-command")] private readonly ICommand _showPerksCommand;

    [NotNull] [Inject(Id = "quit-request-command")] private readonly ICommand _quitRequestCommand;

    public Button NextTurnButton;
    public Button InventoryButton;
    public Button PerksButton;

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

        if (PerksButton != null)
        {
            PerksButton.interactable = _showPerksCommand.CanExecute();
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

    public void ShowPerksButton_Handler()
    {
        if (_playerState.ActiveActor == null)
        {
            return;
        }

        _clientCommandExecutor.Push(_showPerksCommand);
    }

    public void ExitGame_Handler()
    {
        _clientCommandExecutor.Push(_quitRequestCommand);
    }
}
