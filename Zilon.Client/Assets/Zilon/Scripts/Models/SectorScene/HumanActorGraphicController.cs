using UnityEngine;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class HumanActorGraphicController : MonoBehaviour
{
    public IActor Actor { get; set; }
    public ActorGraphicBase Graphic;

    public void Start()
    {
        UpdateEquipment();
        
        Actor.Person.EquipmentCarrier.EquipmentChanged += EquipmentCarrierOnEquipmentChanged;
    }

    private void EquipmentCarrierOnEquipmentChanged(object sender, EquipmentChangedEventArgs e)
    {
        UpdateEquipment();
    }

    private void UpdateEquipment()
    {
        for (var slotIndex = 0; slotIndex < 4; slotIndex++)
        {
            var holder = Graphic.GetVisualProp(slotIndex);
            foreach (Transform visualProp in holder.transform)
            {
                Destroy(visualProp.gameObject);
            }

            VisualProp visualPropResource = null;
            var equipment = Actor.Person.EquipmentCarrier.Equipments[slotIndex];
            if (equipment != null)
            {
                visualPropResource = Resources.Load<VisualProp>($"VisualProps/{equipment.Scheme.Sid}");
            }
            
            if (visualPropResource == null)
            {
                switch (slotIndex)
                {
                    case 2:
                        visualPropResource = Resources.Load<VisualProp>($"VisualProps/steel-armor");
                        break;
                    
                    case 3:
                        visualPropResource = Resources.Load<VisualProp>($"VisualProps/steel-helmet");
                        break;
                }
            }

            if (visualPropResource != null)
            {
                var visualProp = Instantiate(visualPropResource, holder.transform);
            }
        }
    }
}
