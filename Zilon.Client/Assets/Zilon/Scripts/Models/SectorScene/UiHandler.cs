using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics;

public class UiHandler : MonoBehaviour {

	[Inject] private ISectorManager _sectorManager;

	[Inject] private IPlayerState _playerState;
	
	[Inject] private ICommandManager _clientCommandExecutor;
	
	[Inject(Id = "next-turn-command")] private ICommand _nextTurnCommand;
	
	[Inject(Id = "show-inventory-command")] private ICommand _showInventoryCommand;
	
	[Inject(Id = "show-perks-command")] private ICommand _showPerksCommand;
	

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
