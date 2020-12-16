using UnityEngine;

using Zenject;

public class GameLoopUpdaterObject : MonoBehaviour
{
    [Inject]
    private readonly GameLoopUpdater _gameLoopUpdater;

    // Start is called before the first frame update
    public void Start()
    {
        if (!_gameLoopUpdater.IsStarted)
        {
            _gameLoopUpdater.Start();
        }
    }

    public void Destroy()
    {
        if (_gameLoopUpdater.IsStarted)
        {
            _gameLoopUpdater.Stop();
        }
    }

}
