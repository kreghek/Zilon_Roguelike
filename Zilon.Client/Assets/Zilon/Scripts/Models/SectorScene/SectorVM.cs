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
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Services.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

// ReSharper disable once CheckNamespace
// ReSharper disable once ArrangeTypeModifiers
// ReSharper disable once ClassNeverInstantiated.Global
class SectorVM : MonoBehaviour
{
    private MoveCommand _moveCommand;
    private AttackCommand _attackCommand;

#pragma warning disable 649
    // ReSharper disable once NotNullMemberIsNotInitialized
    // ReSharper disable once MemberCanBePrivate.Global
    [NotNull] public MapNodeVM MapNodePrefab;
#pragma warning restore 649

#pragma warning disable 649
    // ReSharper disable once NotNullMemberIsNotInitialized
    // ReSharper disable once MemberCanBePrivate.Global
    [NotNull] public ActorVM ActorPrefab;
#pragma warning restore 649

    [Inject] private ICommandManager _clientCommandExecutor;

    [Inject] private ISectorManager _sectorManager;

    [Inject] private IPlayerState _playerState;

    // ReSharper disable once UnusedMember.Local
    private void FixedUpdate()
    {
        ExecuteCommands();
    }

    private void ExecuteCommands()
    {
        var command = _clientCommandExecutor?.Pop();

        command?.Execute();
    }

    // ReSharper disable once UnusedMember.Local
    private void Awake()
    {
        var mapGenerator = new GridMapGenerator(15);
        var map = new HexMap();
        mapGenerator.CreateMap(map);
        map.Edges.RemoveAt(10);
        map.Edges.RemoveAt(20);
        map.Edges.RemoveAt(30);
        
        var actorManager = new ActorList();
        
        var sector = new Sector(map, actorManager);

        var nodeVMs = new List<MapNodeVM>();
        foreach (var node in map.Nodes)
        {
            var mapNodeVm = Instantiate(MapNodePrefab, transform);

            var hexNode = (HexNode) node;
            var nodeWorldPositionParts = HexHelper.ConvertToWorld(hexNode.OffsetX, hexNode.OffsetY);
            var worldPosition = new Vector3(nodeWorldPositionParts[0], nodeWorldPositionParts[1]);
            mapNodeVm.transform.position = worldPosition;
            mapNodeVm.Node = hexNode;

            var edges = map.Edges.Where(x => x.Nodes.Contains(node)).ToArray();
            var neighbors = (from edge in edges
                from neighbor in edge.Nodes
                where neighbor != node
                select neighbor).Cast<HexNode>().ToArray();

            mapNodeVm.Edges = edges;
            mapNodeVm.Neighbors = neighbors;

            mapNodeVm.OnSelect += MapNodeVm_OnSelect;

            nodeVMs.Add(mapNodeVm);
        }

        var humanPlayer = new HumanPlayer();
        var botPlayer = new BotPlayer();

        var playerActorStartNode = map.Nodes.Cast<HexNode>().Single(n => n.OffsetX == 0 && n.OffsetY == 0);
        var playerActorVm = CreateActorVm(humanPlayer, sector, playerActorStartNode, nodeVMs);

        var enemy1StartNode = map.Nodes.Cast<HexNode>().Single(n => n.OffsetX == 5 && n.OffsetY == 5);
        var enemy1ActorVm = CreateActorVm(botPlayer, sector, enemy1StartNode, nodeVMs);
        enemy1ActorVm.IsEnemy = true;
        enemy1ActorVm.OnSelected += EnemyActorVm_OnSelected;

        var enemy2StartNode = map.Nodes.Cast<HexNode>().Single(n => n.OffsetX == 9 && n.OffsetY == 9);
        var enemy2ActorVm = CreateActorVm(botPlayer, sector, enemy2StartNode, nodeVMs);
        enemy2ActorVm.IsEnemy = true;
        enemy2ActorVm.OnSelected += EnemyActorVm_OnSelected;

        var playerActorTaskSource = new HumanActorTaskSource(playerActorVm.Actor);

        var patrolRoute1 = new PatrolRoute(new IMapNode[]
        {
            map.Nodes.Cast<HexNode>().SingleOrDefault(x=>x.OffsetX == 2 && x.OffsetY == 2),
            map.Nodes.Cast<HexNode>().SingleOrDefault(x=>x.OffsetX == 2 && x.OffsetY == 10)
        });
        
        var patrolRoute2 = new PatrolRoute(new IMapNode[]
        {
            map.Nodes.Cast<HexNode>().SingleOrDefault(x=>x.OffsetX == 10 && x.OffsetY == 2),
            map.Nodes.Cast<HexNode>().SingleOrDefault(x=>x.OffsetX == 10 && x.OffsetY == 10)
        });
        
        
        var routeDictionary = new Dictionary<IActor, IPatrolRoute>()
        {
            {enemy1ActorVm.Actor, patrolRoute1},
            {enemy2ActorVm.Actor, patrolRoute2}
        };
        
        var dice = new Dice();
        var dicisionSource = new DecisionSource(dice);
        var botActorTaskSource = new MonsterActorTaskSource(humanPlayer, 
            routeDictionary,
            dicisionSource);
        
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

        var nodeVm = sender as MapNodeVM;

        _playerState.SelectedNode = nodeVm;

        if (nodeVm != null)
        {
            _clientCommandExecutor.Push(_moveCommand);
        }
    }
}