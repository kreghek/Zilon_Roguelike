using UnityEngine;
using Zilon.Core.Components;

public class VisualPropHolder : MonoBehaviour
{
    public EquipmentSlotTypes SlotTypes;

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
