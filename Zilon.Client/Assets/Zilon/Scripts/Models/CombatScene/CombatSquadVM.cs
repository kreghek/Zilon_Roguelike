using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zilon.Core.Tactics;

public class CombatSquadVM : MonoBehaviour
{
    public ActorSquad ActorSquad { get; set; }
    public List<CombatActorVM> Actors { get; set; }

    public event EventHandler OnSelect;
    public event EventHandler OnNodeChanged;

    public CombatSquadVM()
    {
        Actors = new List<CombatActorVM>();
    }

    public void Start()
    {
        ActorSquad.NodeChanged += ActorSquad_NodeChanged;
    }

    private void ActorSquad_NodeChanged(object sender, EventArgs e)
    {
        OnNodeChanged?.Invoke(this, new EventArgs());
    }

    public void AddActor(CombatActorVM actor)
    {
        Actors.Add(actor);
        actor.OnSelected += Actor_OnSelected;
    }

    private void Actor_OnSelected(object sender, EventArgs e)
    {
        OnSelect?.Invoke(this, new EventArgs());
    }

    public Task<bool> MoveActorsAsync(CombatLocationVM nodeVM)
    {
        var promise = new TaskCompletionSource<bool>();

        var task = promise.Task;

        var actorMoveTasks = new List<Task<bool>>();
        foreach (var actor in Actors)
        {
            var positionOffset = UnityEngine.Random.insideUnitCircle * 2;
            var locationPosition = nodeVM.transform.position;
            var targetPosition = locationPosition + new Vector3(positionOffset.x, positionOffset.y);
            var actorMoveTask = actor.MoveToPointAsync(targetPosition);
            actorMoveTasks.Add(actorMoveTask);
        }

        var squadTask = Task.WhenAll(actorMoveTasks).;

        return squadTask;
    }
}
