using Assets.Zilon.Scripts.Models.Commands;
using Assets.Zilon.Scripts.Models.SectorScene;
using UnityEngine;
using Zenject;
using Zilon.Core.Commands;

public class UiHandler : MonoBehaviour {

	[Inject] private ISectorManager _sectorManager;

	[Inject] private IPlayerState _playerState;
	
	[Inject] private ICommandManager _clientCommandExecutor;
	
	[Inject(Id = "next-turn-command")] private ICommand _nextTurnCommand;
	
	[Inject(Id = "show-inventory-command")] private ICommand _showInventoryCommand;
	

	public void NextTurn()
	{
		_clientCommandExecutor.Push(_nextTurnCommand);
	}

	public void ShowInventoryButton_Handler()
	{
		_clientCommandExecutor.Push(_showInventoryCommand);
	}
}
