using System;
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
    private bool updateFree;

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
	async Task FixedUpdate () {

        if (updateFree)
        {
            return;
        }

        if (_humanActorTaskSource.CurrentActor != null)
        {
            Debug.Log("Request");

            updateFree = true;

            await _gameManager.RequestNextActorTaskAsync();
            //var completionSource = new TaskCompletionSource<IActorTask[]>();

            //var asyncTask = completionSource.Task;
            //completionSource.SetResult(new IActorTask[0]);
            //await asyncTask;

            Debug.Log("Update");

            updateFree = false;
        }
	}
}
