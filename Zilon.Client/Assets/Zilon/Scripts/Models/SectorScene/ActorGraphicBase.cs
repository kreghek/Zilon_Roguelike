using System;
using UnityEngine;

public class ActorGraphicBase : MonoBehaviour
{
    public virtual VisualPropHolder GetVisualProp(int slotIndex)
    {
        throw new NotImplementedException();
    }

    public virtual void ProcessDeath()
    {
        throw new NotImplementedException();
    }
}