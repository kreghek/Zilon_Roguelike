using System.Collections.Generic;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

public sealed class PlayerPersonCreator : MonoBehaviour
{
    [NotNull] [Inject] private readonly ISchemeService _schemeService;

    [NotNull] [Inject] private readonly IPropFactory _propFactory;

    [NotNull] [Inject] private readonly HumanPlayer _humanPlayer;

    [NotNull] [Inject] private readonly ISurvivalRandomSource _survivalRandomSource;

    public HumanPerson CreatePlayerPerson()
    {

        var personScheme = _schemeService.GetScheme<IPersonScheme>("human-person");

        var inventory = new Inventory();

        var evolutionData = new EvolutionData(_schemeService);

        var defaultActScheme = _schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

        var person = new HumanPerson(personScheme, defaultActScheme, evolutionData, _survivalRandomSource, inventory);

        _humanPlayer.MainPerson = person;


        var classRoll = Random.Range(1, 6);
        switch (classRoll)
        {
            case 1:
                AddEquipmentToActor(person.EquipmentCarrier, 2, "short-sword");
                AddEquipmentToActor(person.EquipmentCarrier, 1, "steel-armor");
                AddEquipmentToActor(person.EquipmentCarrier, 3, "wooden-shield");
                break;

            case 2:
                AddEquipmentToActor(person.EquipmentCarrier, 2, "battle-axe");
                AddEquipmentToActor(person.EquipmentCarrier, 3, "battle-axe");
                AddEquipmentToActor(person.EquipmentCarrier, 0, "highlander-helmet");
                break;

            case 3:
                AddEquipmentToActor(person.EquipmentCarrier, 2, "bow");
                AddEquipmentToActor(person.EquipmentCarrier, 1, "leather-jacket");
                AddEquipmentToActor(inventory, "short-sword");
                AddResourceToActor(inventory, "arrow", 10);
                break;

            case 4:
                AddEquipmentToActor(person.EquipmentCarrier, 2, "fireball-staff");
                AddEquipmentToActor(person.EquipmentCarrier, 1, "scholar-robe");
                AddEquipmentToActor(person.EquipmentCarrier, 0, "wizard-hat");
                AddResourceToActor(inventory, "mana", 15);
                break;

            case 5:
                AddEquipmentToActor(person.EquipmentCarrier, 2, "pistol");
                AddEquipmentToActor(person.EquipmentCarrier, 0, "elder-hat");
                AddResourceToActor(inventory, "bullet-45", 5);

                AddResourceToActor(inventory, "packed-food", 1);
                AddResourceToActor(inventory, "water-bottle", 1);
                AddResourceToActor(inventory, "med-kit", 1);

                AddResourceToActor(inventory, "mana", 5);
                AddResourceToActor(inventory, "arrow", 3);
                break;
        }

        AddResourceToActor(inventory, "packed-food", 1);
        AddResourceToActor(inventory, "water-bottle", 1);
        AddResourceToActor(inventory, "med-kit", 1);

        return person;
    }

    private void AddEquipmentToActor(Inventory inventory, string equipmentSid)
    {
        try
        {
            var equipmentScheme = _schemeService.GetScheme<IPropScheme>(equipmentSid);
            var equipment = _propFactory.CreateEquipment(equipmentScheme);
            inventory.Add(equipment);
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError($"Не найден объект {equipmentSid}");
        }
    }

    private void AddEquipmentToActor(IEquipmentCarrier equipmentCarrier, int slotIndex, string equipmentSid)
    {
        try
        {
            var equipmentScheme = _schemeService.GetScheme<IPropScheme>(equipmentSid);
            var equipment = _propFactory.CreateEquipment(equipmentScheme);
            equipmentCarrier[slotIndex] = equipment;
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError($"Не найден объект {equipmentSid}");
        }
    }

    private void AddResourceToActor(Inventory inventory, string resourceSid, int count)
    {
        try
        {
            var resourceScheme = _schemeService.GetScheme<IPropScheme>(resourceSid);
            var resource = _propFactory.CreateResource(resourceScheme, count);
            inventory.Add(resource);
        }
        catch (KeyNotFoundException)
        {
            Debug.LogError($"Не найден объект {resourceSid}");
        }
    }

}
