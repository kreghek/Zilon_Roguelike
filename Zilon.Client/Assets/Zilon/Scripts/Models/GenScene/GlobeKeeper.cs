using UnityEngine;

using Zilon.Core.World;

public class GlobeKeeper : MonoBehaviour
{
    public GlobeInitiatorHandler GlobeInitiatorHandler;
    public Globe Globe { get; private set; }

    private async void Start()
    {
        Globe = await GlobeInitiatorHandler.GenerateGlobeAsync().ConfigureAwait(false);
    }

    public bool HasGlobe { get => Globe != null; }
}
