using System.Collections.Generic;
using UnityEngine;

//TODO Переименованить в просто Актёр, потому что актёры есть только в бою
public class CombatSquadVM : MonoBehaviour
{
    public List<CombatActorVM> Actors { get; set; }

    public CombatSquadVM()
    {
        Actors = new List<CombatActorVM>();
    }
}
