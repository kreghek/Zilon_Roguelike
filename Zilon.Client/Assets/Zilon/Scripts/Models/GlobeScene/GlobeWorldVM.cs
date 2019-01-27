using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.UI;

using Zenject;
using Zilon.Core.Common;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

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

        var locationNodeViewModels = new List<MapLocation>(100);
        foreach (GlobeRegionNode globeRegionNode in region.Nodes)
        {
            var worldCoords = HexHelper.ConvertToWorld(globeRegionNode.OffsetX, globeRegionNode.OffsetY);

            var locationObject = _container.InstantiatePrefab(LocationPrefab, transform);
            locationObject.transform.position = new Vector3(worldCoords[0], worldCoords[1], 0);
            var locationViewModel = locationObject.GetComponent<MapLocation>();
            locationViewModel.Node = globeRegionNode;
            locationNodeViewModels.Add(locationViewModel);
        }

        var openNodeViewModels = new List<MapLocation>(locationNodeViewModels);
        while (openNodeViewModels.Any())
        {
            var currentNodeViewModel = openNodeViewModels[0];
            openNodeViewModels.Remove(currentNodeViewModel);

            var neighbors = region.GetNext(currentNodeViewModel.Node);
            var neighborViewModels = openNodeViewModels.Where(x => neighbors.Contains(x.Node)).ToArray();
            foreach (var neibourNodeViewModel in neighborViewModels)
            {
                var connectorObject = _container.InstantiatePrefab(ConnectorPrefab, transform);
                var connectorViewModel = connectorObject.GetComponent<MapLocationConnector>();
                connectorViewModel.gameObject1 = currentNodeViewModel.gameObject;
                connectorViewModel.gameObject2 = neibourNodeViewModel.gameObject;
                //openNodeViewModels.Remove(neibourNodeViewModel);
            }
        }
    }
}