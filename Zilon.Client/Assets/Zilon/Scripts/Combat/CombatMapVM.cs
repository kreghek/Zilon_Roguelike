using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zilon.Logic.Persons;
using Zilon.Logic.Players;
using Zilon.Logic.Services;
using Zilon.Logic.Tactics;
using Zilon.Logic.Tactics.Initialization;
using Zilon.Logic.Tactics.Map;

public class CombatMapVM : MonoBehaviour
{

    public CombatLocationVM LocationPrefab;
    public CombatPathVM PathPrefab;
    public CombatActorVM ActorPrefab;
    public Canvas Canvas;

    private List<CombatLocationVM> locations;

    private readonly CombatService combatService;

    private void Awake()
    {
        var initData = GetTwoGroupsData();
        var combat = combatService.CreateCombat(initData);
        CreateLocations(combat);
        CreateActors(combat);
    }

    private void CreateLocations(Combat combat)
    {
        locations = new List<CombatLocationVM>();

        foreach (var mapNode in combat.Map.Nodes)
        {
            var locationobj = Instantiate(LocationPrefab, transform);
            locationobj.transform.position = new Vector3(mapNode.Position.X, mapNode.Position.Y);
            locations.Add(locationobj);
        }
    }

    private void CreateActors(Combat combat)
    {
        foreach (var actor in combat.Actors)
        {
            var actorObj = Instantiate(ActorPrefab, transform);
            var positionOffset = UnityEngine.Random.insideUnitCircle * 2;
            var locationPosition = new Vector3(actor.CurrentNode.Position.X, actor.CurrentNode.Position.Y);
            actorObj.transform.position = locationPosition + new Vector3(positionOffset.x, positionOffset.y);
        }
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
