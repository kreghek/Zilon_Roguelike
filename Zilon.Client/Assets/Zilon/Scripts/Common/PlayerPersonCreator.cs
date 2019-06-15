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
                AddEquipment(person.EquipmentCarrier, 2, "short-sword");
                AddEquipment(person.EquipmentCarrier, 1, "steel-armor");
                AddEquipment(person.EquipmentCarrier, 3, "wooden-shield");
                break;

            case 2:
                AddEquipment(person.EquipmentCarrier, 2, "battle-axe");
                AddEquipment(person.EquipmentCarrier, 3, "battle-axe");
                AddEquipment(person.EquipmentCarrier, 0, "highlander-helmet");
                break;

            case 3:
                AddEquipment(person.EquipmentCarrier, 2, "bow");
                AddEquipment(person.EquipmentCarrier, 1, "leather-jacket");
                AddEquipment(inventory, "short-sword");
                AddResource(inventory, "arrow", 10);
                break;

            case 4:
                AddEquipment(person.EquipmentCarrier, 2, "fireball-staff");
                AddEquipment(person.EquipmentCarrier, 1, "scholar-robe");
                AddEquipment(person.EquipmentCarrier, 0, "wizard-hat");
                AddResource(inventory, "mana", 15);
                break;

            case 5:
                AddEquipment(person.EquipmentCarrier, 2, "pistol");
                AddEquipment(person.EquipmentCarrier, 0, "elder-hat");
                AddResource(inventory, "bullet-45", 5);

                AddResource(inventory, "packed-food", 1);
                AddResource(inventory, "water-bottle", 1);
                AddResource(inventory, "med-kit", 1);

                AddResource(inventory, "mana", 5);
                AddResource(inventory, "arrow", 3);
                break;
        }

        AddResource(inventory, "packed-food", 1);
        AddResource(inventory, "water-bottle", 1);
        AddResource(inventory, "med-kit", 1);

        return person;
    }

    public void AddResourceToCurrentPerson(string resourceSid, int count = 1)
    {
        var inventory = (Inventory)_humanPlayer.MainPerson.Inventory;
        AddResource(inventory, resourceSid, count);
    }

    private void AddEquipment(Inventory inventory, string equipmentSid)
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

    private void AddEquipment(IEquipmentCarrier equipmentCarrier, int slotIndex, string equipmentSid)
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

    private void AddResource(Inventory inventory, string resourceSid, int count)
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
