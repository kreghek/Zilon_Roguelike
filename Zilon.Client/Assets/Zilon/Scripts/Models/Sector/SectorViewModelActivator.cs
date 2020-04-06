using UnityEngine;

public class SectorViewModelActivator : MonoBehaviour
{
    public SectorViewModel TargetSectorViewModel;

    public void Update()
    {
        // Вместо этих строк нужно брать доменную модель сектора из HumanPlayer/SectorManager

        //var sectorInfo = globe.SectorInfos.First();
        //var sector = sectorInfo.Sector;

        //TargetSectorViewModel.Init(sector);

        Destroy(this);
    }
}
