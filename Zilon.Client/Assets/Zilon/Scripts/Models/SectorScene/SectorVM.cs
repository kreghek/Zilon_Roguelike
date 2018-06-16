using System;
using System.Linq;
using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Models.Commands;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Core.Commands;
using Zilon.Core.Persons;
using Zilon.Core.Services;
using Zilon.Core.Services.CombatEvents;
using Zilon.Core.Services.CombatMap;
using Zilon.Core.Services.MapGenerators;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Events;
using Zilon.Core.Tactics.Map;

class SectorVM : MonoBehaviour
{

    private HumanBehaviourSource _playerBehaviourSource;
    private Sector _sector;
    
    private float turnCounter;

    public SchemeLocator SchemeLocator;
    public Text Text;
    
    public MapNodeVM MapNodePrefab;
    public ActorVM ActorPrefab;

    [Inject]
    private ICommandManager _commandManager;
//    [Inject]
//    private ICombatService _combatService;
    [Inject]
    private ICombatManager _combatManager;
    [Inject]
    private IEventManager _eventManager;
    [Inject]
    private IMapGenerator _mapGenerator;

//    [Inject(Id = "squad-command-factory")]
//    private ICommandFactory _commandFactory;

    private void FixedUpdate()
    {
//        ExecuteCommands();
//        UpdateEvents();
//        UpdateTurnCounter();
    }

//    private void UpdateTurnCounter()
//    {
//        turnCounter += Time.deltaTime;
//        if (turnCounter < 10)
//        {
//            return;
//        }
//
//        turnCounter = 0;
//
//        var endTurnCommand = _commandFactory.CreateCommand<EndTurnCommand>();
//        _commandManager.Push(endTurnCommand);
//    }

    private void UpdateEvents()
    {
       // _eventManager.Update();
    }

    private void ExecuteCommands()
    {
//        var command = _commandManager?.Pop();
//        if (command == null)
//            return;
//
//        Debug.Log($"Executing {command}");
//
//        command.Execute();
    }

    private void Awake()
    {
//        var initData = CombatHelper.GetData(_mapGenerator);
//        var combat = _combatService.CreateCombat(initData);
//        _combatManager.CurrentCombat = combat;
        
        var mapGenerator = new GridMapGenerator();
        var map = new CombatMap();
        mapGenerator.CreateMap(map);
        var sector = new Sector(map);

        foreach (var node in map.Nodes)
        {
            
            var mapNodeVM = Instantiate(MapNodePrefab, transform);
            
            var position = new Vector3(node.Position.X, node.Position.Y);
            mapNodeVM.transform.position = position;
            mapNodeVM.Node = node;
            
            mapNodeVM.OnSelect+= MapNodeVmOnOnSelect;
        }

        var playerPerson = new Person();
        
        var playerActor = sector.AddActor(playerPerson, map.Nodes.First());

        var playerActorVM = Instantiate(ActorPrefab, transform);
        var actorPosition = new Vector3(playerActor.Node.Position.X, playerActor.Node.Position.Y);
        playerActorVM.transform.position = actorPosition;
        playerActorVM.Actor = playerActor;

        _playerBehaviourSource = new HumanBehaviourSource(playerActor);
        sector.BehaviourSources = new IBehaviourSource[] { _playerBehaviourSource };

        _sector = sector;

        //Map.InitCombat();
    }

    private void MapNodeVmOnOnSelect(object sender, EventArgs e)
    {
        // указываем намерение двигиться на выбранную точку (узел).
        
        var nodeVM = sender as MapNodeVM;

        if (nodeVM != null)
        {
            var targetNode = nodeVM.Node;
            _playerBehaviourSource.AssignMoveToPointCommand(targetNode);
            _sector.Update();
        }
    }
}
