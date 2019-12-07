using System.Threading.Tasks;

using UnityEngine;

using Zenject;

using Zilon.Core.World;

public class GlobeInitiatorHandler : MonoBehaviour
{
    [Inject] private readonly IGlobeGenerator _globeGenerator;

    public async Task<Globe> GenerateGlobeAsync()
    {
        var result = await _globeGenerator.CreateGlobeAsync();
        return result.Globe;
    }
}
