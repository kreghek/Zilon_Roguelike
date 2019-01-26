using System.Linq;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;
using Zilon.Core.Common;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;

public class GlobeWorldVM : MonoBehaviour
{
    public MapLocation LocationPrefab;
    public MapLocationConnector ConnectorPrefab;

    [Inject] private readonly IGlobeManager _globeManager;
    [Inject] private readonly HumanPlayer _player;
    [Inject] private readonly DiContainer _container;

    
    void Start()
    {
        if (_globeManager.CurrentGlobe == null)
        {
            _globeManager.GenerateGlobe();
        }

        var currentLocality = _globeManager.CurrentGlobe.Localities.First();
        var currentGlobeCell = currentLocality.Cell;

        var region = _globeManager.GenerateRegion(currentGlobeCell);

        foreach (HexNode globeRegionNode in region.Nodes)
        {
            var worldCoords = HexHelper.ConvertToWorld(globeRegionNode.OffsetX, globeRegionNode.OffsetY);

            var locationObject = _container.InstantiatePrefab(LocationPrefab, transform);
            locationObject.transform.position = new Vector3(worldCoords[0], worldCoords[1], 0);
        }
    }
}