using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Models.Commands;
using Assets.Zilon.Scripts.Models.SectorScene;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Persons;
using Zilon.Core.Services.CombatEvents;
using Zilon.Core.Services.CombatMap;
using Zilon.Core.Services.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

class SectorVM : MonoBehaviour
{
    private MoveCommand _moveCommand;
    private AttackCommand _attackCommand;
    
    private float turnCounter;

    public SchemeLocator SchemeLocator;
    public Text Text;
    
    public MapNodeVM MapNodePrefab;
    public ActorVM ActorPrefab;

    [Inject]
    private ICommandManager _clientCommandExecutor;

    [Inject]
    private ISectorManager _sectorManager;
    
    [Inject]
    private IPlayerState _playerState;

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
       
        var mapGenerator = new GridMapGenerator();
        var map = new Map();
        mapGenerator.CreateMap(map);
        var sector = new Sector(map);

        var nodeVMs = new List<MapNodeVM>();
        foreach (var node in map.Nodes)
        {
            
            var mapNodeVM = Instantiate(MapNodePrefab, transform);

            var nodeWorldPositionParts = HexHelper.ConvertToWorld(node.OffsetX, node.OffsetY);
            var worldPosition = new Vector3(nodeWorldPositionParts[0], nodeWorldPositionParts[1]);
            mapNodeVM.transform.position = worldPosition;
            mapNodeVM.Node = node;
            
            mapNodeVM.OnSelect+= MapNodeVm_OnSelect;

            nodeVMs.Add(mapNodeVM);
        }



        var playerActorStartNode = map.Nodes.Single(n => n.OffsetX == 0 && n.OffsetY == 0);
        var playerActorVM = CreateActorVm(sector, playerActorStartNode, nodeVMs);

        var enemy1StartNode = map.Nodes.Single(n => n.OffsetX == 5 && n.OffsetY == 5);
        var enemy1ActorVM = CreateActorVm(sector, enemy1StartNode, nodeVMs);
        enemy1ActorVM.IsEnemy = true;
        enemy1ActorVM.OnSelected += EnemyActorVm_OnSelected;
        
        var enemy2StartNode = map.Nodes.Single(n => n.OffsetX == 9 && n.OffsetY == 9);
        var enemy2ActorVM = CreateActorVm(sector, enemy2StartNode, nodeVMs);
        enemy2ActorVM.IsEnemy = true;
        enemy2ActorVM.OnSelected += EnemyActorVm_OnSelected;

        var playerActorTaskSource = new HumanActorTaskSource(playerActorVM.Actor);
        sector.BehaviourSources = new IActorTaskSource[] { playerActorTaskSource };

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

    private ActorVM CreateActorVm(Sector sector, HexNode playerActorStartNode, List<MapNodeVM> nodeVMs)
    {
        var person = new Person
        {
            Hp = 1,
            Damage = 1
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
