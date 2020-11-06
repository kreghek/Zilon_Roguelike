using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
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
        if (_humanPlayer.SectorNode.Sector is null)
        {
            // Это может происходить, пока создаётся сектор при Awake модели представления сектора.
            // TODO Добавить метод Init для того, чтобы сообщить текущему обработчику,
            // что можно начать мониторинг.
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
