using System;
using System.Collections.Generic;
using UnityEngine;
using Zilon.Logic.Tactics;

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

    internal void MoveActors(CombatLocationVM nodeVM)
    {
        foreach (var actor in Actors)
        {
            var positionOffset = UnityEngine.Random.insideUnitCircle * 2;
            var locationPosition = nodeVM.transform.position;
            var targetPosition = locationPosition + new Vector3(positionOffset.x, positionOffset.y);
            actor.ChangeTargetPosition(targetPosition);
        }
    }
}
