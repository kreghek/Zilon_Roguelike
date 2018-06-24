using Assets.Zilon.Scripts.Models.Commands;
using Assets.Zilon.Scripts.Models.SectorScene;
using UnityEngine;
using Zenject;
using Zilon.Core.Commands;

public class UiHandler : MonoBehaviour {

	[Inject] private ISectorManager _sectorManager;

	[Inject] private IPlayerState _playerState;
	
	[Inject] private ICommandManager _clientCommandExecutor;
	
	private NextTurnCommand _nextTurnCommand;

	private void Awake()
	{
		_nextTurnCommand = new NextTurnCommand(_sectorManager, _playerState);	
	}
	
	public void NextTurn()
	{
		_clientCommandExecutor.Push(_nextTurnCommand);
	}
}
