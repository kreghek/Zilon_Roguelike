using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.World;

public class SectorViewModelActivator : MonoBehaviour
{
    [Inject]
    private readonly IGlobeManager _globeManager;

    public SectorViewModel TargetSectorViewModel;

    public void Update()
    {
        if (!_globeManager.IsGlobeInitialized)
        {
            return;
        }

        var globe = _globeManager.Globe;

        //var sectorInfo = globe.SectorInfos.First();
        //var sector = sectorInfo.Sector;

        //TargetSectorViewModel.Init(sector);

        Destroy(this);
    }
}
