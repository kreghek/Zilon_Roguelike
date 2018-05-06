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
    private readonly ICommandManager commandManager;

    private Combat combat;

    public CombatMapVM(CombatService combatService, ICommandManager commandManager)
    {
        this.commandManager = commandManager;
        this.combatService = combatService;
    }

    public void InitCombat(Combat combat)
    {
        this.combat = combat;
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
                var moveCommand = new MoveCommand(selectedSquad, sender as CombatLocationVM);
                commandManager.Push((ICommand<ICommandContext>)moveCommand);
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
