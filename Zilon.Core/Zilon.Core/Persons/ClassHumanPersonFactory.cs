using System.Collections.Generic;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.PersonModules;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public sealed class ClassHumanPersonFactory : IHumanPersonFactory
    {
        private readonly IDice _dice;
        private readonly ISchemeService _schemeService;
        private readonly ISurvivalRandomSource _survivalRandomSource;
        private readonly IPropFactory _propFactory;

        public ClassHumanPersonFactory(IDice dice,
            ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory)
        {
            _dice = dice;
            _schemeService = schemeService ?? throw new System.ArgumentNullException(nameof(schemeService));
            _survivalRandomSource = survivalRandomSource ?? throw new System.ArgumentNullException(nameof(survivalRandomSource));
            _propFactory = propFactory ?? throw new System.ArgumentNullException(nameof(propFactory));
        }

        public HumanPerson Create()
        {
            var personScheme = _schemeService.GetScheme<IPersonScheme>("human-person");

            var inventory = new Inventory();

            var evolutionData = new EvolutionData(_schemeService);

            var defaultActScheme = _schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme, defaultActScheme, evolutionData, _survivalRandomSource, inventory);

            var equipmentModule = person.GetModule<IEquipmentModule>();

            var classRoll = _dice.RollD6();
            switch (classRoll)
            {
                case 1:
                    AddEquipment(equipmentModule, 2, "short-sword");
                    AddEquipment(equipmentModule, 1, "steel-armor");
                    AddEquipment(equipmentModule, 3, "wooden-shield");
                    break;

                case 2:
                    AddEquipment(equipmentModule, 2, "battle-axe");
                    AddEquipment(equipmentModule, 3, "battle-axe");
                    AddEquipment(equipmentModule, 0, "highlander-helmet");
                    break;

                case 3:
                    AddEquipment(equipmentModule, 2, "bow");
                    AddEquipment(equipmentModule, 1, "leather-jacket");
                    AddEquipment(inventory, "short-sword");
                    AddResource(inventory, "arrow", 10);
                    break;

                case 4:
                    AddEquipment(equipmentModule, 2, "fireball-staff");
                    AddEquipment(equipmentModule, 1, "scholar-robe");
                    AddEquipment(equipmentModule, 0, "wizard-hat");
                    AddResource(inventory, "mana", 15);
                    break;

                case 5:
                    AddEquipment(equipmentModule, 2, "pistol");
                    AddEquipment(equipmentModule, 0, "elder-hat");
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

        private void AddEquipment(Inventory inventory, string equipmentSid)
        {
            try
            {
                var equipmentScheme = _schemeService.GetScheme<IPropScheme>(equipmentSid);
                var equipment = _propFactory.CreateEquipment(equipmentScheme);
                inventory.Add(equipment);
            }
            catch (KeyNotFoundException exception)
            {
                throw new CreatePersonException($"Не найден объект {equipmentSid}", exception);
            }
        }

        private void AddEquipment(IEquipmentModule equipmentCarrier, int slotIndex, string equipmentSid)
        {
            try
            {
                var equipmentScheme = _schemeService.GetScheme<IPropScheme>(equipmentSid);
                var equipment = _propFactory.CreateEquipment(equipmentScheme);
                equipmentCarrier[slotIndex] = equipment;
            }
            catch (KeyNotFoundException exception)
            {
                throw new CreatePersonException($"Не найден объект {equipmentSid}", exception);
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
            catch (KeyNotFoundException exception)
            {
                throw new CreatePersonException($"Не найден объект {resourceSid}", exception);
            }
        }
    }
}
