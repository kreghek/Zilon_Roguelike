using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Zilon.Scripts.Services;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Commands.Globe;
using Zilon.Core.Common;
using Zilon.Core.Players;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

public class GlobeWorldVM : MonoBehaviour
{
    private bool _interuptCommands;

    public MapLocation LocationPrefab;
    public MapLocationConnector ConnectorPrefab;
    public GroupVM HumanGroupPrefab;
    public GlobalFollowCamera Camera;
    public SceneLoader SectorSceneLoader;
    public SceneLoader GlobeSceneLoader;

    private GroupVM _groupViewModel;
    private GlobeRegion _region;
    private List<MapLocation> _locationNodeViewModels;

    [Inject] private readonly IWorldManager _globeManager;
    [Inject] private readonly IWorldGenerator _globeGenerator;
    [Inject] private readonly HumanPlayer _player;
    [Inject] private readonly DiContainer _container;
    [Inject] private readonly MoveGroupCommand _moveGroupCommand;
    [Inject] private readonly ICommandManager _clientCommandExecutor;
    [Inject] private readonly ICommandBlockerService _commandBlockerService;
    [Inject] private readonly IGlobeUiState _globeUiState;

    public PlayerPersonCreator PersonCreator;

    private async void Start()
    {
        if (_globeManager.Globe == null)
        {
            _globeManager.Globe = await _globeGenerator.GenerateGlobeAsync();

            var firstLocality = _globeManager.Globe.Localities.First();

            _player.Terrain = firstLocality.Cell;

            var createdRegion = await _globeGenerator.GenerateRegionAsync(_globeManager.Globe, firstLocality.Cell);

            _globeManager.Regions[_player.Terrain] = createdRegion;

            var firstNode = (GlobeRegionNode)createdRegion.Nodes.First();

            _player.GlobeNode = firstNode;

            firstNode.ObservedState = GlobeNodeObservedState.Visited;
        }

        // Создание соседних регионов
        for (var offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (var offsetY = -1; offsetY <= 1; offsetY++)
            {
                if (offsetX == 0 && offsetY == 0)
                {
                    // Это нулевое смещение от текущего элемента.
                    // Пропускаем, т.к. текущий элемент уже есть.
                    continue;
                }

                var terrainX = _player.Terrain.Coords.X + offsetX;
                var terrainY = _player.Terrain.Coords.Y + offsetY;

                if (_globeManager.Globe.Terrain.GetLowerBound(0) <= terrainX &&
                    terrainX <= _globeManager.Globe.Terrain.GetUpperBound(0) &&
                    _globeManager.Globe.Terrain[0].GetLowerBound(0) <= terrainY &&
                    terrainY <= _globeManager.Globe.Terrain[0].GetUpperBound(0))
                {
                    var terrainCell = _globeManager.Globe.Terrain[terrainX][terrainY];
                    if (!_globeManager.Regions.ContainsKey(terrainCell))
                    {
                        var createdNeiborRegion = await _globeGenerator.GenerateRegionAsync(_globeManager.Globe, terrainCell);

                        _globeManager.Regions[terrainCell] = createdNeiborRegion;
                    }
                }
            }
        }

        var currentGlobeCell = _player.Terrain;
        _region = _globeManager.Regions[currentGlobeCell];


        // Создание визуализации узлов провинции.
        _locationNodeViewModels = new List<MapLocation>(100);
        foreach (GlobeRegionNode globeRegionNode in _region.Nodes)
        {
            var worldCoords = HexHelper.ConvertToWorld(globeRegionNode.OffsetX + _player.Terrain.Coords.X * 20, 
                globeRegionNode.OffsetY + _player.Terrain.Coords.Y * 20);

            var locationObject = _container.InstantiatePrefab(LocationPrefab, transform);
            locationObject.transform.position = new Vector3(worldCoords[0], worldCoords[1], 0);
            var locationViewModel = locationObject.GetComponent<MapLocation>();
            locationViewModel.Node = globeRegionNode;
            locationViewModel.ParentRegion = _region;
            _locationNodeViewModels.Add(locationViewModel);

            locationViewModel.OnSelect += LocationViewModel_OnSelect;
            locationViewModel.OnHover += LocationViewModel_OnHover;
        }

        // Создание коннекторов между узлами провинции.
        var openNodeViewModels = new List<MapLocation>(_locationNodeViewModels);
        while (openNodeViewModels.Any())
        {
            var currentNodeViewModel = openNodeViewModels[0];
            openNodeViewModels.Remove(currentNodeViewModel);

            var neighbors = _region.GetNext(currentNodeViewModel.Node);
            var neighborViewModels = openNodeViewModels.Where(x => neighbors.Contains(x.Node)).ToArray();
            foreach (var neibourNodeViewModel in neighborViewModels)
            {
                var connectorObject = _container.InstantiatePrefab(ConnectorPrefab, transform);
                var connectorViewModel = connectorObject.GetComponent<MapLocationConnector>();
                connectorViewModel.gameObject1 = currentNodeViewModel.gameObject;
                connectorViewModel.gameObject2 = neibourNodeViewModel.gameObject;

                currentNodeViewModel.NextNodes.Add(neibourNodeViewModel);
                neibourNodeViewModel.NextNodes.Add(currentNodeViewModel);

                currentNodeViewModel.Connectors.Add(connectorViewModel);
                neibourNodeViewModel.Connectors.Add(connectorViewModel);

                connectorViewModel.gameObject.SetActive(false);
            }
        }

        // Создание визуализаций соседних провинций
        var currentRegion = _globeManager.Regions[_player.Terrain];
        var currentBorderNodes = currentRegion.Nodes.OfType<GlobeRegionNode>().Where(x => x.IsBorder).ToArray();
        for (var offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (var offsetY = -1; offsetY <= 1; offsetY++)
            {
                if (offsetX == 0 && offsetY == 0)
                {
                    // Это нулевое смещение от текущего элемента.
                    // Пропускаем, т.к. текущий элемент уже есть.
                    continue;
                }

                var terrainX = _player.Terrain.Coords.X + offsetX;
                var terrainY = _player.Terrain.Coords.Y + offsetY;

                if (_globeManager.Globe.Terrain.GetLowerBound(0) <= terrainX &&
                    terrainX <= _globeManager.Globe.Terrain.GetUpperBound(0) &&
                    _globeManager.Globe.Terrain[0].GetLowerBound(0) <= terrainY &&
                    terrainY <= _globeManager.Globe.Terrain[0].GetUpperBound(0))
                {
                    var terrainCell = _globeManager.Globe.Terrain[terrainX][terrainY];

                    var neighborRegion = _globeManager.Regions[terrainCell];

                    // Ищем узел текущей провинции, являющийся соседним с узлом соседней провинции.
                    var neighborBorderNodes = neighborRegion.Nodes.OfType<GlobeRegionNode>().Where(x => x.IsBorder);

                    foreach (var neighborBorderNode in neighborBorderNodes)
                    {

                        var worldCoords = HexHelper.ConvertToWorld(neighborBorderNode.OffsetX + terrainX * 20,
                            neighborBorderNode.OffsetY + terrainY * 20);

                        var locationObject = _container.InstantiatePrefab(LocationPrefab, transform);
                        locationObject.transform.position = new Vector3(worldCoords[0], worldCoords[1], 0);
                        var locationViewModel = locationObject.GetComponent<MapLocation>();
                        locationViewModel.Node = neighborBorderNode;
                        locationViewModel.ParentRegion = neighborRegion;
                        locationViewModel.OtherRegion = true;
                        _locationNodeViewModels.Add(locationViewModel);

                        locationViewModel.OnSelect += LocationViewModel_OnSelect;
                        locationViewModel.OnHover += LocationViewModel_OnHover;
                    }
                }
            }
        }

        if (_player.MainPerson == null)
        {
            _player.MainPerson = PersonCreator.CreatePlayerPerson();
        }

        var playerGroupNodeViewModel = _locationNodeViewModels.Single(x => x.Node == _player.GlobeNode);
        var groupObject = _container.InstantiatePrefab(HumanGroupPrefab, transform);
        _groupViewModel = groupObject.GetComponent<GroupVM>();
        _groupViewModel.CurrentLocation = playerGroupNodeViewModel;
        groupObject.transform.position = playerGroupNodeViewModel.transform.position;
        Camera.Target = groupObject;

        _player.GlobeNodeChanged += HumanPlayer_GlobeNodeChanged;
        MoveGroupViewModel(_player.GlobeNode);
    }

    private void LocationViewModel_OnHover(object sender, EventArgs e)
    {
        _globeUiState.HoverViewModel = sender as MapLocation;
    }

    public void OnDestroy()
    {
        _player.GlobeNodeChanged -= HumanPlayer_GlobeNodeChanged;
    }

    private void FixedUpdate()
    {
        if (!_commandBlockerService.HasBlockers)
        {
            ExecuteCommands();
        }
    }

    private void ExecuteCommands()
    {
        var command = _clientCommandExecutor.Pop();

        try
        {
            if (command != null)
            {
                command.Execute();

                if (_interuptCommands)
                {
                    return;
                }

                if (command is IRepeatableCommand repeatableCommand)
                {
                    if (repeatableCommand.CanRepeat())
                    {
                        _clientCommandExecutor.Push(repeatableCommand);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Не удалось выполнить команду {command}.", exception);
        }
    }

    private void HumanPlayer_GlobeNodeChanged(object sender, EventArgs e)
    {
        if (_player.GlobeNode == null)
        {
            return;
        }

        if (_player.GlobeNode.Scheme.SectorLevels != null || _player.GlobeNode.IsTown)
        {
            if (!_player.GlobeNode.IsHome)
            {
                StartLoadScene();
            }
            else
            {
                MoveGroupViewModel(_player.GlobeNode);
                Debug.Log("Home");
            }
        }
        else
        {
            if (_player.GlobeNode.MonsterState != null)
            {
                StartLoadScene();
            }

            MoveGroupViewModel(_player.GlobeNode);

            if (!_region.Nodes.Contains(_player.GlobeNode))
            {
                // Значит провинция сменилась.
                GlobeSceneLoader.LoadScene();
            }
        }
    }

    private void MoveGroupViewModel(GlobeRegionNode targetNode)
    {
        var selectedNodeViewModel = GetNodeViewModel(targetNode);
        _groupViewModel.CurrentLocation = selectedNodeViewModel;
    }

    private MapLocation GetNodeViewModel(GlobeRegionNode targetNode)
    {
        var locationViewModel = _locationNodeViewModels.Single(x => x.Node == targetNode);
        return locationViewModel;
    }

    private void LocationViewModel_OnSelect(object sender, EventArgs e)
    {
        if (_moveGroupCommand.CanExecute())
        {
            _globeUiState.SelectedViewModel = (MapLocation)sender;
            _clientCommandExecutor.Push(_moveGroupCommand);

            var currentRegion = _globeManager.Regions[_player.Terrain];
            _globeManager.UpdateRegionNodes(currentRegion);
        }
        else
        {
            Debug.Log("Попытка перемещения с критическим уровнем голода/жажды.");
        }
    }

    private void StartLoadScene()
    {
        SectorSceneLoader.LoadScene();
    }
}