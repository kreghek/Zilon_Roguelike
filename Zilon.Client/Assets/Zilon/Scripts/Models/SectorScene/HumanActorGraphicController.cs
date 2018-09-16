using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class HumanActorGraphicController : MonoBehaviour
{
    private readonly Dictionary<int, VisualPropHolder> _visualSlots;

    public IActor Actor { get; set; }
    public ActorGraphicBase Graphic;

    public HumanActorGraphicController()
    {
        _visualSlots = new Dictionary<int, VisualPropHolder>();
    }

    public void Start()
    {
        ProjectSlotsToVisual();

        UpdateEquipment();

        Actor.Person.EquipmentCarrier.EquipmentChanged += EquipmentCarrierOnEquipmentChanged;
    }

    private void ProjectSlotsToVisual()
    {
        var humanHumanoidGraphic = (HumanoidActorGraphic)Graphic;

        var visualHolderList = humanHumanoidGraphic.VisualHolders.ToList();

        var equipmentCarrier = Actor.Person.EquipmentCarrier;
        for (var slotIndex = 0; slotIndex < equipmentCarrier.Slots.Length; slotIndex++)
        {
            var slot = equipmentCarrier.Slots[slotIndex];
            var visualHolder = visualHolderList.FirstOrDefault(x => (x.SlotTypes & slot.Types) > 0);
            if (visualHolder != null)
            {
                _visualSlots[slotIndex] = visualHolder;
            }
        }
    }

    private void EquipmentCarrierOnEquipmentChanged(object sender, EquipmentChangedEventArgs e)
    {
        UpdateEquipment();
    }

    private void UpdateEquipment()
    {
        var equipmentCarrier = Actor.Person.EquipmentCarrier;
        for (var slotIndex = 0; slotIndex < equipmentCarrier.Slots.Length; slotIndex++)
        {
            var slot = equipmentCarrier.Slots[slotIndex];
            var types = slot.Types;

            VisualPropHolder holder;
            if (_visualSlots.TryGetValue(slotIndex, out holder))
            {
                foreach (Transform visualProp in holder.transform)
                {
                    Destroy(visualProp.gameObject);
                }

                VisualProp visualPropResource = null;
                var equipment = equipmentCarrier.Equipments[slotIndex];
                if (equipment != null)
                {
                    visualPropResource = Resources.Load<VisualProp>($"VisualProps/{equipment.Scheme.Sid}");
                }

                if (visualPropResource != null)
                {
                    var visualProp = Instantiate(visualPropResource, holder.transform);
                }
            }



            //VisualProp visualPropResource = null;
            //var equipment = equipmentCarrier.Equipments[slotIndex];
            //if (equipment != null)
            //{
            //    visualPropResource = Resources.Load<VisualProp>($"VisualProps/{equipment.Scheme.Sid}");
            //}
            
            //if (visualPropResource == null)
            //{
            //    switch (slotIndex)
            //    {
            //        case 2:
            //            visualPropResource = Resources.Load<VisualProp>($"VisualProps/steel-armor");
            //            break;
                    
            //        case 3:
            //            visualPropResource = Resources.Load<VisualProp>($"VisualProps/steel-helmet");
            //            break;
            //    }
            //}

            
        }
    }
}
