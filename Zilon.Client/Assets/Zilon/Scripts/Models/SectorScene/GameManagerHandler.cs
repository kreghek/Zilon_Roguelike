using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

public class GameManagerHandler : MonoBehaviour
{

    [Inject] private IHumanActorTaskSource _humanActorTaskSource;
	[Inject] private IGameManager _gameManager;
	[Inject] private IDecisionSource _decisionSource;
	
	// Use this for initialization
	void Start ()
	{
		_gameManager.ActorTaskSources = new[]
		{
            _humanActorTaskSource
        };
	}
	
	// Update is called once per frame
	async Task Update () {
		await _gameManager.RequestNextActorTaskAsync();
		Debug.Log("Update");
	}
}
