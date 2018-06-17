using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Zilon.Scripts.Models.Commands;
using Assets.Zilon.Scripts.Models.SectorScene;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Services.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

class SectorVM : MonoBehaviour
{
    private MoveCommand _moveCommand;
    private AttackCommand _attackCommand;

    public MapNodeVM MapNodePrefab;
    public ActorVM ActorPrefab;

    [Inject] private ICommandManager _clientCommandExecutor;

    [Inject] private ISectorManager _sectorManager;

    [Inject] private IPlayerState _playerState;

    private void FixedUpdate()
    {
        ExecuteCommands();
    }

    private void ExecuteCommands()
    {
        var command = _clientCommandExecutor?.Pop();

        command?.Execute();
    }

    private void Awake()
    {
        var mapGenerator = new GridMapGenerator(15);
        var map = new HexMap();
        mapGenerator.CreateMap(map);
        map.Edges.RemoveAt(10);
        map.Edges.RemoveAt(20);
        map.Edges.RemoveAt(30);
        
        var sector = new Sector(map);

        var nodeVMs = new List<MapNodeVM>();
        foreach (var node in map.Nodes)
        {
            var mapNodeVM = Instantiate(MapNodePrefab, transform);

            var hexNode = (HexNode) node;
            var nodeWorldPositionParts = HexHelper.ConvertToWorld(hexNode.OffsetX, hexNode.OffsetY);
            var worldPosition = new Vector3(nodeWorldPositionParts[0], nodeWorldPositionParts[1]);
            mapNodeVM.transform.position = worldPosition;
            mapNodeVM.Node = hexNode;

            var edges = map.Edges.Where(x => x.Nodes.Contains(node)).ToArray();
            var neighbors = (from edge in edges
                from neighbor in edge.Nodes
                where neighbor != node
                select neighbor).Cast<HexNode>().ToArray();

            mapNodeVM.Edges = edges;
            mapNodeVM.Neighbors = neighbors;

            mapNodeVM.OnSelect += MapNodeVm_OnSelect;

            nodeVMs.Add(mapNodeVM);
        }

        var humanPlayer = new HumanPlayer();
        var botPlayer = new BotPlayer();

        var playerActorStartNode = map.Nodes.Cast<HexNode>().Single(n => n.OffsetX == 0 && n.OffsetY == 0);
        var playerActorVM = CreateActorVm(humanPlayer, sector, playerActorStartNode, nodeVMs);

        var enemy1StartNode = map.Nodes.Cast<HexNode>().Single(n => n.OffsetX == 5 && n.OffsetY == 5);
        var enemy1ActorVM = CreateActorVm(botPlayer, sector, enemy1StartNode, nodeVMs);
        enemy1ActorVM.IsEnemy = true;
        enemy1ActorVM.OnSelected += EnemyActorVm_OnSelected;

        var enemy2StartNode = map.Nodes.Cast<HexNode>().Single(n => n.OffsetX == 9 && n.OffsetY == 9);
        var enemy2ActorVM = CreateActorVm(botPlayer, sector, enemy2StartNode, nodeVMs);
        enemy2ActorVM.IsEnemy = true;
        enemy2ActorVM.OnSelected += EnemyActorVm_OnSelected;

        var playerActorTaskSource = new HumanActorTaskSource(playerActorVM.Actor);
        
        var botActorTaskSource = new BotActorTaskSource(botPlayer);
        
        sector.BehaviourSources = new IActorTaskSource[]
        {
            playerActorTaskSource,
            botActorTaskSource
        };

        _sectorManager.CurrentSector = sector;

        _playerState.TaskSource = playerActorTaskSource;

        _moveCommand = new MoveCommand(_sectorManager, _playerState);
        _attackCommand = new AttackCommand(_sectorManager, _playerState);
    }

    private void EnemyActorVm_OnSelected(object sender, EventArgs e)
    {
        var actorVm = sender as ActorVM;

        _playerState.SelectedActor = actorVm;

        if (actorVm != null)
        {
            _clientCommandExecutor.Push(_attackCommand);
        }
    }

    private ActorVM CreateActorVm(IPlayer player, [NotNull] Sector sector,
        [NotNull] HexNode playerActorStartNode,
        [NotNull] List<MapNodeVM> nodeVMs)
    {
        var person = new Person
        {
            Hp = 1,
            Damage = 1,
            Player = player
        };
        var actor = sector.AddActor(person, playerActorStartNode);

        var actorVm = Instantiate(ActorPrefab, transform);

        var actorNodeVm = nodeVMs.Single(x => x.Node == actor.Node);
        var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
        actorVm.transform.position = actorPosition;
        actorVm.Actor = actor;
        return actorVm;
    }

    private void MapNodeVm_OnSelect(object sender, EventArgs e)
    {
        // указываем намерение двигиться на выбранную точку (узел).

        var nodeVM = sender as MapNodeVM;

        _playerState.SelectedNode = nodeVM;

        if (nodeVM != null)
        {
            _clientCommandExecutor.Push(_moveCommand);
        }
    }
}