using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Zilon.Scripts.Models.Commands;
using Assets.Zilon.Scripts.Services;
using UnityEngine;
using Zilon.Logic.Persons;
using Zilon.Logic.Players;
using Zilon.Logic.Services;
using Zilon.Logic.Tactics;
using Zilon.Logic.Tactics.Initialization;
using Zilon.Logic.Tactics.Map;

class CombatMapVM : MonoBehaviour
{

    public CombatLocationVM LocationPrefab;
    public CombatPathVM PathPrefab;
    public CombatSquadVM SquadPrefab;
    public CombatActorVM ActorPrefab;
    public Canvas Canvas;

    private List<CombatLocationVM> locations;
    private CombatSquadVM selectedSquad;

    private readonly CombatService combatService;
    private ICommandManager commandManager;
    private Combat combat;

    public void SetCommandManager(ICommandManager commandManager)
    {
        this.commandManager = commandManager;
    }

    private void Awake()
    {
        var initData = GetTwoGroupsData();
        combat = combatService.CreateCombat(initData);
        CreateLocations(combat);
        CreateActors(combat);
    }

    private void CreateLocations(Combat combat)
    {
        locations = new List<CombatLocationVM>();

        foreach (var node in combat.Map.Nodes)
        {
            var locationVM = Instantiate(LocationPrefab, transform);
            locationVM.Node = node;
            locationVM.transform.position = new Vector3(node.Position.X, node.Position.Y);
            locations.Add(locationVM);

            locationVM.OnSelect += LocationVM_OnSelect;
        }
    }

    private void LocationVM_OnSelect(object sender, EventArgs e)
    {
        if (selectedSquad != null)
        {
            if (combat != null && commandManager != null)
            {
                var moveCommand = new MoveCommand(combat, selectedSquad, sender as CombatLocationVM);
                commandManager.Push(moveCommand);
            }
        }
    }

    private void CreateActors(Combat combat)
    {
        foreach (var squad in combat.Squads)
        {
            var squadVM = Instantiate(SquadPrefab, transform);
            squadVM.ActorSquad = squad;

            var currentSquadNode = locations.SingleOrDefault(x => x.Node == squad.Node);
            if (currentSquadNode != null)
            {
                foreach (var actor in squad.Actors)
                {
                    var actorVM = Instantiate(ActorPrefab, squadVM.transform);
                    var positionOffset = UnityEngine.Random.insideUnitCircle * 2;
                    var locationPosition = currentSquadNode.transform.position;
                    actorVM.transform.position = locationPosition + new Vector3(positionOffset.x, positionOffset.y);
                    actorVM.ChangeTargetPosition(actorVM.transform.position);

                    squadVM.AddActor(actorVM);
                }
            }

            squadVM.OnSelect += SquadVM_OnSelect;
        }
    }

    private void SquadVM_OnSelect(object sender, EventArgs e)
    {
        selectedSquad = sender as CombatSquadVM;
        Debug.Log("selected " + selectedSquad);
    }

    private static CombatInitData GetTwoGroupsData()
    {
        return new CombatInitData
        {
            Map = new CombatMap(),
            Players = new[] {
                    new PlayerCombatInitData{
                        Player = CreateFakePlayer(),
                        Squads =new[]{
                            CreateSquad(5),
                            CreateSquad(3),
                            CreateSquad(4)
                        }
                    },
                    new PlayerCombatInitData{
                        Player = CreateFakePlayer(),
                        Squads =new[]{
                            CreateSquad(5),
                            CreateSquad(3),
                            CreateSquad(4),
                            CreateSquad(6),
                        }
                    }
                }
        };
    }

    private static Person CreatePerson()
    {
        var person = new Person { };
        return person;
    }

    private static IPlayer CreateFakePlayer()
    {
        return new HumanPlayer();
    }

    private static Squad CreateSquad(int count)
    {
        var persons = new List<Person>();

        for (var i = 0; i < count; i++)
        {
            persons.Add(CreatePerson());
        }

        return new Squad
        {
            Persons = persons.ToArray()
        };
    }

    public CombatMapVM()
    {
        combatService = new CombatService();
    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
