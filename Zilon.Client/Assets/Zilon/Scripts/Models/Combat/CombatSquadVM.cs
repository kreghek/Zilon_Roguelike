using System.Collections.Generic;
using UnityEngine;
using Zilon.Logic.Tactics;

//TODO Переименованить в просто Актёр, потому что актёры есть только в бою
public class CombatSquadVM : MonoBehaviour
{
    public ActorSquad ActorSquad { get; set; }
    public List<CombatActorVM> Actors { get; set; }

    public CombatSquadVM()
    {
        Actors = new List<CombatActorVM>();
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
