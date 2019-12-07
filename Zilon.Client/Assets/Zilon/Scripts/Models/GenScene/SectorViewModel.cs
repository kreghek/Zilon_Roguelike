using System.Threading.Tasks;

using UnityEngine;

using Zenject;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

public class SectorViewModel : MonoBehaviour
{
    [Inject] private readonly IGlobeGenerator _globeGenerator;
    [Inject(Id = "monster")] private readonly IActorTaskSource _actorTaskSource;

    public SectorMapViewModel MapViewModel;
    public ActorsViewModel ActorsViewModel;
    public GlobeKeeper GlobeKeeper;

    public ISector Sector { get; private set; }
    public bool IsInitialized { get; private set; }

    public async Task Init(ISector sector)
    {
        Sector = sector;

        MapViewModel.Init(Sector.Map);

        ActorsViewModel.Init(Sector.ActorManager);

        IsInitialized = true;
    }
}
