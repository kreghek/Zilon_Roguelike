using UnityEngine;

using Zenject;

using Zilon.Core.Client.Sector;

public class GameLoopUpdaterObject : MonoBehaviour
{
    [Inject]
    private readonly IGameLoopUpdater _gameLoopUpdater;

    // Start is called before the first frame update
    public void Start()
    {
        if (!_gameLoopUpdater.IsStarted)
        {
            _gameLoopUpdater.Start();
        }
    }
}