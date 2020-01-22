using System.Threading.Tasks;

using UnityEngine;

using Zilon.Core.Tactics;

public class SectorViewModel : MonoBehaviour
{
    public SectorMapViewModel MapViewModel;
    public ActorsViewModel ActorsViewModel;

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
