using UnityEngine;

using Zenject;

using Zilon.Core.Client.Sector;

public class GlobeLoopUpdaterObject : MonoBehaviour
{
    [Inject]
    private readonly IGlobeLoopUpdater _globeLoopUpdater;

    public void Start()
    {
        if (!_globeLoopUpdater.IsStarted)
        {
            _globeLoopUpdater.Start();
        }
    }
}