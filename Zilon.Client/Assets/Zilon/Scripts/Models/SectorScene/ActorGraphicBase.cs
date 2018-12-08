using System;

using UnityEngine;

using Zilon.Core.Components;

public class ActorGraphicBase : MonoBehaviour
{
    public virtual VisualPropHolder GetVisualProp(EquipmentSlotTypes types)
    {
        throw new NotImplementedException();
    }

    public virtual void ProcessDeath(GameObject rootObject, bool rootRotting)
    {
        var corpse = gameObject.AddComponent<CorpseActivity>();
        corpse.RootObject = rootObject;
        corpse.RootRotting = rootRotting;

        Destroy(gameObject.GetComponent<PersonActivity>());
    }
}