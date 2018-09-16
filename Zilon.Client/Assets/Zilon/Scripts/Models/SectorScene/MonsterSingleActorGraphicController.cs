using UnityEngine;

using Zilon.Core.Tactics;

public class MonsterSingleActorGraphicController : MonoBehaviour
{
    public IActor Actor { get; set; }
    public ActorGraphicBase Graphic;
    
    public void Start()
    {
        SetVisualProp("moon-rat", 0);
    }

    private void SetVisualProp(string propSid, int slotIndex)
    {
        var monoGraphics = (MonoActorGraphic)Graphic;
        var holder = monoGraphics.VisualPropHolder; // Graphic.GetVisualProp(slotIndex);
        var visualPropResource = Resources.Load<VisualProp>($"VisualProps/{propSid}");
        Instantiate(visualPropResource, holder.transform);
    }
}
