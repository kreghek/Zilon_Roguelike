using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics;

public class UiHandler : MonoBehaviour
{

    [Inject] private readonly ISectorManager _sectorManager;

    [Inject] private readonly IPlayerState _playerState;

    [Inject] private readonly ICommandManager _clientCommandExecutor;

    [Inject(Id = "next-turn-command")] private readonly ICommand _nextTurnCommand;

    [Inject(Id = "show-inventory-command")] private readonly ICommand _showInventoryCommand;

    [Inject(Id = "show-perks-command")] private readonly ICommand _showPerksCommand;


    public void NextTurn()
    {
        _clientCommandExecutor.Push(_nextTurnCommand);
    }

    public void ShowInventoryButton_Handler()
    {
        _clientCommandExecutor.Push(_showInventoryCommand);
    }

    public void ShowPerksButton_Handler()
    {
        _clientCommandExecutor.Push(_showPerksCommand);
    }
}
