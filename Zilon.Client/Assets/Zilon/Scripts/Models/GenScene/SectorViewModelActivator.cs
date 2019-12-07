using System.Linq;

using UnityEngine;

public class SectorViewModelActivator : MonoBehaviour
{
    public GlobeKeeper GlobeKeeper;
    public SectorViewModel TargetSectorViewModel;

    async void Update()
    {
        if (!GlobeKeeper.HasGlobe)
        {
            return;
        }

        var globe = GlobeKeeper.Globe;

        var sectorInfo = globe.SectorInfos.First();
        var sector = sectorInfo.Sector;

        await TargetSectorViewModel.Init(sector);

        Destroy(this);
    }
}
