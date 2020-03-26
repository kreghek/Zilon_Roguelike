using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics;

public class RestIndicatorHandler : MonoBehaviour
{
    public GameObject Icon;

    [Inject]
    private readonly HumanPlayer _humanPlayer;

    [Inject]
    private readonly ISectorUiState _playerState;

    public void Start()
    {
        Icon.SetActive(false);
    }

    public void Update()
    {
        var actorManager = _humanPlayer.SectorNode.Sector.ActorManager;
        if (actorManager.Items.Any(x => x != _playerState.ActiveActor?.Actor))
        {
            Icon.SetActive(false);
        }
        else
        {
            Icon.SetActive(true);
        }
    }
}
