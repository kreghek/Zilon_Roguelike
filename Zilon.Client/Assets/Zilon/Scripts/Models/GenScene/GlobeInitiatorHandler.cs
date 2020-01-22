using System.Threading.Tasks;

using UnityEngine;

using Zenject;

using Zilon.Core.World;

public class GlobeInitiatorHandler : MonoBehaviour
{
    [Inject] private readonly IGlobeManager _globeManager;

    private async void Start()
    {
        await InitializeGlobeAsync();
    }

    private async Task InitializeGlobeAsync()
    {
        await _globeManager.InitializeGlobeAsync();
    }
}
