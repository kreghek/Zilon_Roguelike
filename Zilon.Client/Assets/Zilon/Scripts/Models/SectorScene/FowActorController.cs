using Assets.Zilon.Scripts.Models.SectorScene;

using UnityEngine;

using Zilon.Core.Tactics;

public class FowActorController : MonoBehaviour, IFowObjectController
{
    public GameObject Graphic;

    public Collider2D Collider;

    public void ChangeState(SectorMapNodeFowState state)
    {
        if (Graphic == null)
        {
            return;
        }

        Graphic.gameObject.SetActive(state == SectorMapNodeFowState.Observing);

        if (Collider != null)
        {
            Collider.enabled = Graphic.gameObject.activeSelf;
        }
    }
}
