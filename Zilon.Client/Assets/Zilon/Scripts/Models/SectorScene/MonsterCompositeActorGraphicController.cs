using UnityEngine;

using Zilon.Core.Tactics;

public class MonsterCompositeActorGraphicController : MonoBehaviour
{
    public IActor Actor { get; set; }
    public ActorGraphicBase Graphic;
    
    //public void Start()
    //{
    //    SetVisualProp("steel-armor", 2);
    //    SetVisualProp("steel-helmet", 3);
    //}

    //private void SetVisualProp(string propSid, int slotIndex)
    //{
    //    var holder = Graphic.GetVisualProp(slotIndex);
    //    var visualPropResource = Resources.Load<VisualProp>($"VisualProps/{propSid}");
    //    Instantiate(visualPropResource, holder.transform);
    //}
}
