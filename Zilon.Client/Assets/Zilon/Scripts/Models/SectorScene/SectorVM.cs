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
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Generation;
using Zilon.Core.Tactics.Spatial;

// ReSharper disable once CheckNamespace
// ReSharper disable once ArrangeTypeModifiers
// ReSharper disable once ClassNeverInstantiated.Global
class SectorVM : MonoBehaviour
{
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
    
    [Inject] private IDecisionSource _decisionSource;

    [Inject] private ISectorGeneratorRandomSource _sectorGeneratorRandomSource;
    
    [Inject] private ISchemeService _schemeService;
    
    [Inject] private IPropFactory _propFactory;

    [Inject(Id = "move-command")] private ICommand _moveCommand;

    [Inject(Id = "attack-command")] private ICommand _attackCommand;


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
        var humanPlayer = new HumanPlayer();
        var botPlayer = new BotPlayer();
        
        var map = new HexMap();
        
        var actorManager = new ActorList();
        
        var sector = new Sector(map, actorManager);
        
        var sectorGenerator = new SectorProceduralGenerator(_sectorGeneratorRandomSource, botPlayer);

        try
        {
            sectorGenerator.Generate(sector, map);
        }
        catch (Exception)
        {
            Debug.Log(sectorGenerator.Log.ToString());
            throw;
        }
        
        Debug.Log(sectorGenerator.Log.ToString());

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

        var propScheme = _schemeService.GetScheme<PropScheme>("short-sword");

        var playerEquipment = _propFactory.CreateEquipment(propScheme);
        var playerActorStartNode = sectorGenerator.StartNodes.First();
        var playerActorVm = CreateActorVm(humanPlayer, actorManager, playerActorStartNode, nodeVMs, playerEquipment);

        foreach (var monsterActor in sectorGenerator.MonsterActors)   {
            actorManager.Add(monsterActor);

            var actorVm = Instantiate(ActorPrefab, transform);

            var actorNodeVm = nodeVMs.Single(x => x.Node == monsterActor.Node);
            var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
            actorVm.transform.position = actorPosition;
            actorVm.Actor = monsterActor;
            actorVm.IsEnemy = true;
            actorVm.OnSelected += EnemyActorVm_OnSelected;
        }

        var playerActorTaskSource = new HumanActorTaskSource(playerActorVm.Actor,_decisionSource);

        var botActorTaskSource = new MonsterActorTaskSource(botPlayer, 
            sectorGenerator.Patrols,
            _decisionSource);
        
        sector.BehaviourSources = new IActorTaskSource[]
        {
            playerActorTaskSource,
            botActorTaskSource
        };

        _sectorManager.CurrentSector = sector;

        _playerState.TaskSource = playerActorTaskSource;
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

    private ActorVM CreateActorVm(IPlayer player,
        [NotNull] IActorManager actorManager,
        [NotNull] IMapNode startNode,
        [NotNull] IEnumerable<MapNodeVM> nodeVMs,
        [NotNull] Equipment equipment)
    {
        var person = new Person
        {
            Hp = 1
        };
        
        person.EquipmentCarrier.SetEquipment(equipment, 0);

        if (player is HumanPlayer)
        {
            person.Hp = 10;
        }

        var actor = new Actor(person, player, startNode);
        
        actorManager.Add(actor);

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