using System.Collections.Generic;
using System.Linq;
using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Services;
using Assets.Zilon.Scripts.Services.CombatScene;
using UnityEngine;
using Zenject;
using Zilon.Logic.Services;

class CombatMapVM : MonoBehaviour
{

    public CombatLocationVM LocationPrefab;
    public CombatPathVM PathPrefab;
    public CombatSquadVM SquadPrefab;
    public CombatActorVM ActorPrefab;
    public Canvas Canvas;

    private List<CombatLocationVM> locations;

    [Inject]
    public ICommandManager CommandManager;
    [Inject]
    public ICombatService CombatService;
    [Inject]
    private IPersonCommandHandler PersonCommandHandler;
    [Inject]
    private ICombatManager CombatManager;

    public void InitCombat()
    {
        CreateLocations();
        CreateActors();
    }


    private void CreateLocations()
    {
        locations = new List<CombatLocationVM>();

        foreach (var node in CombatManager.CurrentCombat.Map.Nodes)
        {
            var locationVM = Instantiate(LocationPrefab, transform);
            locationVM.Node = node;
            locationVM.transform.position = new Vector3(node.Position.X, node.Position.Y);
            locations.Add(locationVM);

            locationVM.OnSelect += PersonCommandHandler.LocationVM_OnSelect;
        }
    }

    

    private void CreateActors()
    {
        foreach (var squad in CombatManager.CurrentCombat.Squads)
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

            squadVM.OnSelect += PersonCommandHandler.SquadVM_OnSelect;
        }
    }
}
