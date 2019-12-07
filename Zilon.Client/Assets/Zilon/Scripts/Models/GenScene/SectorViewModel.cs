using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Tactics;
using Zilon.Core.World;

public class SectorViewModel : MonoBehaviour
{
    [Inject] private readonly IGlobeGenerator _globeGenerator;

    private Globe _globe;
    private ISector _sector;
    private IActor _followedActor;

    private float _counter;

    private async void Start()
    {
        var globeGenerationResult = await _globeGenerator.CreateGlobeAsync();

        _globe = globeGenerationResult.Globe;

        var sectorInfo = _globe.SectorInfos.First();
        _sector = sectorInfo.Sector;

        _followedActor = _sector.ActorManager.Items.First();
    }

    // Update is called once per frame
    void Update()
    {
        const float TURN_DURATION = 2f;

        _counter += Time.deltaTime;
        if (_counter < TURN_DURATION)
        {
            return;
        }

        _counter -= TURN_DURATION;
    }
}
