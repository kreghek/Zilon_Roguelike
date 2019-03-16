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
    public SceneLoader SceneLoader;

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
        }

        var currentGlobeCell = _player.Terrain;
        _region = _globeManager.Regions[currentGlobeCell];

        _locationNodeViewModels = new List<MapLocation>(100);
        foreach (GlobeRegionNode globeRegionNode in _region.Nodes)
        {
            var worldCoords = HexHelper.ConvertToWorld(globeRegionNode.OffsetX, globeRegionNode.OffsetY);

            var locationObject = _container.InstantiatePrefab(LocationPrefab, transform);
            locationObject.transform.position = new Vector3(worldCoords[0], worldCoords[1], 0);
            var locationViewModel = locationObject.GetComponent<MapLocation>();
            locationViewModel.Node = globeRegionNode;
            _locationNodeViewModels.Add(locationViewModel);

            locationViewModel.OnSelect += LocationViewModel_OnSelect;
        }

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
            }
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
            StartLoadScene();
        }
        else
        {
            MoveGroupViewModel(_player.GlobeNode);
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
        _globeUiState.SelectedViewModel = (MapLocation)sender;
        _clientCommandExecutor.Push(_moveGroupCommand);
    }

    private void StartLoadScene()
    {
        SceneLoader.gameObject.SetActive(true);
    }
}