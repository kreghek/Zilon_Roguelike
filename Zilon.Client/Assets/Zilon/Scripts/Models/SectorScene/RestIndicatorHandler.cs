using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

public class RestIndicatorHandler : MonoBehaviour
{
    public GameObject Icon;

    [Inject]
    private readonly IActorManager _actorManager;

    [Inject]
    private readonly ISectorUiState _playerState;

    public void Start()
    {
        Icon.SetActive(false);
    }

    public void Update()
    {
        if (_actorManager.Items.Any(x => x != _playerState.ActiveActor?.Actor))
        {
            Icon.SetActive(false);
        }
        else
        {
            Icon.SetActive(true);
        }
    }
}
