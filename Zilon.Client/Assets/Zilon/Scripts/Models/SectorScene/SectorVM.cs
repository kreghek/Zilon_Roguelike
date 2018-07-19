using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Zilon.Scripts.Models.SectorScene;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

    public ContainerVm ContainerPrefab;

    public GameObject WindowsParent;

    public GameObject ShowContainerModalPrefab;

    [Inject] private ICommandManager _clientCommandExecutor;

    [Inject] private ISectorManager _sectorManager;

    [Inject] private IPlayerState _playerState;
    
    [Inject] private IDecisionSource _decisionSource;

    [Inject] private ISectorGeneratorRandomSource _sectorGeneratorRandomSource;
    
    [Inject] private ISchemeService _schemeService;
    
    [Inject] private IPropFactory _propFactory;

    [Inject(Id = "move-command")] private ICommand _moveCommand;

    [Inject(Id = "attack-command")] private ICommand _attackCommand;
    
    [Inject(Id = "open-container-command")] private ICommand _openContainerCommand;
    
    [Inject(Id = "show-container-modal-command")] private ICommand _showContainerModalCommand;


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
        CreateSector();
    }

    private void CreateSector()
    {
        var humanPlayer = new HumanPlayer();
        var botPlayer = new BotPlayer();

        var map = new HexMap();

        var actorManager = new ActorManager();
        
        var propContainerManager = new PropContainerManager();

        var sector = new Sector(map, actorManager, propContainerManager);

        var sectorGenerator = new SectorProceduralGenerator(_sectorGeneratorRandomSource, botPlayer, _schemeService);

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

            if (sector.ExitNodes.Contains(node))
            {
                mapNodeVm.IsExit = true;
            }

            mapNodeVm.OnSelect += MapNodeVm_OnSelect;

            nodeVMs.Add(mapNodeVm);
        }

        var propScheme = _schemeService.GetScheme<PropScheme>("pistol");
        var personScheme = _schemeService.GetScheme<PersonScheme>("captain");

        var playerEquipment = _propFactory.CreateEquipment(propScheme);
        var playerActorStartNode = sectorGenerator.StartNodes.First();
        var playerActorVm = CreateActorVm(humanPlayer,
            personScheme,
            actorManager,
            playerActorStartNode,
            nodeVMs,
            playerEquipment);
        playerActorVm.Actor.OpenedContainer += PlayerActorOnOpenedContainer;

        _playerState.ActiveActor = playerActorVm;

        foreach (var monsterActor in sectorGenerator.MonsterActors)
        {
            actorManager.Add(monsterActor);

            var actorVm = Instantiate(ActorPrefab, transform);

            var actorNodeVm = nodeVMs.Single(x => x.Node == monsterActor.Node);
            var actorPosition = actorNodeVm.transform.position + new Vector3(0, 0, -1);
            actorVm.transform.position = actorPosition;
            actorVm.Actor = monsterActor;
            actorVm.IsEnemy = true;
            actorVm.OnSelected += EnemyActorVm_OnSelected;
        }

        foreach (var container in sectorGenerator.Containers)
        {
            var containerVm = Instantiate(ContainerPrefab, transform);
            
            var containerNodeVm = nodeVMs.Single(x => x.Node == container.Node);
            var containerPosition = containerNodeVm.transform.position + new Vector3(0, 0, -1);
            containerVm.transform.position = containerPosition;
            containerVm.Container = container;
            containerVm.Selected += Container_Selected;
        }

        var playerActorTaskSource = new HumanActorTaskSource(playerActorVm.Actor, _decisionSource);

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

        sector.ActorExit += SectorOnActorExit;
    }

    private void Container_Selected(object sender, EventArgs e)
    {
        var containerVm = sender as ContainerVm;

        _playerState.SelectedContainer = containerVm;

        if (containerVm != null)
        {
            _clientCommandExecutor.Push(_openContainerCommand);
        }
    }

    private void PlayerActorOnOpenedContainer(object sender, OpenContainerEventArgs e)
    {
        _clientCommandExecutor.Push(_showContainerModalCommand);
    }

    private void SectorOnActorExit(object sender, EventArgs e)
    {
        SceneManager.LoadScene("combat");
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

    private ActorVM CreateActorVm([NotNull] IPlayer player,
        [NotNull] PersonScheme personScheme,
        [NotNull] IActorManager actorManager,
        [NotNull] IMapNode startNode,
        [NotNull] IEnumerable<MapNodeVM> nodeVMs,
        [NotNull] Equipment equipment)
    {
        var person = new Person(personScheme);
        
        person.EquipmentCarrier.SetEquipment(equipment, 0);

        var inventory = new Inventory();
        var foodScheme = _schemeService.GetScheme<PropScheme>("food-pack");
        var resource = new Resource(foodScheme, 3);
        inventory.Add(resource);
        var actor = new Actor(person, player, startNode, inventory);
        
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