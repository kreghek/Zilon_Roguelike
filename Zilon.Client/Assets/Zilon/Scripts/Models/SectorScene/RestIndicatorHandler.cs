using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;

public class RestIndicatorHandler : MonoBehaviour
{
    public GameObject Icon;

    [Inject]
    private readonly IPlayer _humanPlayer;

    [Inject]
    private readonly ISectorUiState _playerState;

    public void Start()
    {
        Icon.SetActive(false);
    }

    public void Update()
    {
        if (_humanPlayer.MainPerson is null)
        {
            // In start of the game until initialization was not complete yet.
            Icon.SetActive(false);
            return;
        }

        if (_humanPlayer.MainPerson.GetModuleSafe<ISurvivalModule>()?.IsDead == true)
        {
            Icon.SetActive(false);
            return;
        }

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
