using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public sealed class RandomHumanPersonFactory : IHumanPersonFactory
    {
        private const int HEAD_SLOT_INDEX = 0;
        private const int BODY_SLOT_INDEX = 1;
        private const int MAIN_HAND_SLOT_INDEX = 2;
        private const int OFF_HAND_SLOT_INDEX = 3;

        private readonly IDice _dice;
        private readonly ISchemeService _schemeService;
        private readonly ISurvivalRandomSource _survivalRandomSource;
        private readonly IPropFactory _propFactory;
        private readonly IDropResolver _dropResolver;

        public RandomHumanPersonFactory(IDice dice,
            ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            IDropResolver dropResolver)
        {
            _dice = dice;
            _schemeService = schemeService ?? throw new System.ArgumentNullException(nameof(schemeService));
            _survivalRandomSource = survivalRandomSource ?? throw new System.ArgumentNullException(nameof(survivalRandomSource));
            _propFactory = propFactory ?? throw new System.ArgumentNullException(nameof(propFactory));
            _dropResolver = dropResolver ?? throw new System.ArgumentNullException(nameof(dropResolver));
        }

        public HumanPerson Create()
        {
            var personScheme = _schemeService.GetScheme<IPersonScheme>("human-person");

            var inventory = new Inventory();

            var evolutionData = new EvolutionData(_schemeService);

            var defaultActScheme = _schemeService.GetScheme<ITacticalActScheme>(personScheme.DefaultAct);

            var person = new HumanPerson(personScheme, defaultActScheme, evolutionData, _survivalRandomSource, inventory);

            var headDropScheme = GetHeads();
            FillSlot(person, headDropScheme, HEAD_SLOT_INDEX);

            var armorDropScheme = GetArmors();
            FillSlot(person, armorDropScheme, BODY_SLOT_INDEX);

            var mainWeaponDropScheme = GetMainWeapons();
            FillSlot(person, mainWeaponDropScheme, MAIN_HAND_SLOT_INDEX);

            var offWeaponDropScheme = GetOffWeapons();
            FillSlot(person, offWeaponDropScheme, OFF_HAND_SLOT_INDEX);

            var startPropDropScheme = GetStartProps();
            var startProps = _dropResolver.Resolve(new[] { startPropDropScheme });
            foreach (var prop in startProps)
            {
                AddPropToInventory(inventory, prop);
            }

            AddResource(inventory, "packed-food", 1);
            AddResource(inventory, "water-bottle", 1);
            AddResource(inventory, "med-kit", 1);

            return person;
        }

        private void FillSlot(HumanPerson person, IDropTableScheme mainWeaponDropScheme, int slotIndex)
        {
            var inventory = person.Inventory;
            var mainWeaponEquipments = _dropResolver.Resolve(new[] { mainWeaponDropScheme });
            var usedMainWeaponEquipment = mainWeaponEquipments.OfType<Equipment>().FirstOrDefault();
            AddEquipment(person.EquipmentCarrier, slotIndex, usedMainWeaponEquipment);
            var unusedMainWeaponDrops = mainWeaponEquipments.Where(x => x != usedMainWeaponEquipment).ToArray();
            foreach (var prop in unusedMainWeaponDrops)
            {
                AddPropToInventory(inventory, prop);
            }
        }

        private IDropTableScheme GetHeads()
        {
            return _schemeService.GetScheme<IDropTableScheme>("start-heads");
        }

        private IDropTableScheme GetMainWeapons()
        {
            return _schemeService.GetScheme<IDropTableScheme>("start-main-weapons");
        }

        private IDropTableScheme GetArmors()
        {
            return _schemeService.GetScheme<IDropTableScheme>("start-armors");
        }

        private IDropTableScheme GetOffWeapons()
        {
            return _schemeService.GetScheme<IDropTableScheme>("start-off-weapons");
        }

        private IDropTableScheme GetStartProps()
        {
            return _schemeService.GetScheme<IDropTableScheme>("start-off-weapons");
        }

        private void AddEquipment(IEquipmentCarrier equipmentCarrier, int slotIndex, Equipment equipment)
        {
            equipmentCarrier[slotIndex] = equipment;
        }

        private void AddPropToInventory(IPropStore inventory, IProp prop)
        {
            inventory.Add(prop);
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
