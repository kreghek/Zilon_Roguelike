using System.Collections.Generic;
using System.Linq;

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
        private const string HEAD_DROP_SID = "start-heads";
        private const string MAIN_WEAPON_DROP_SID = "start-main-weapons";
        private const string BODY_DROP_SID = "start-armors";
        private const string OFF_WEAPON_DROP_SID = "start-off-weapons";
        private const string START_PROP_DROP_SID = "start-props";
        private readonly ISchemeService _schemeService;
        private readonly ISurvivalRandomSource _survivalRandomSource;
        private readonly IPropFactory _propFactory;
        private readonly IDropResolver _dropResolver;

        public RandomHumanPersonFactory(
            ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            IDropResolver dropResolver)
        {
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

            //var headDropScheme = GetHeads();
            //FillSlot(person, headDropScheme, HEAD_SLOT_INDEX);

            //var armorDropScheme = GetArmors();
            //FillSlot(person, armorDropScheme, BODY_SLOT_INDEX);

            //var mainWeaponDropScheme = GetMainWeapons();
            //FillSlot(person, mainWeaponDropScheme, MAIN_HAND_SLOT_INDEX);

            //var offWeaponDropScheme = GetOffWeapons();
            //FillSlot(person, offWeaponDropScheme, OFF_HAND_SLOT_INDEX);

            //var startPropDropScheme = GetStartProps();
            //var startProps = _dropResolver.Resolve(new[] { startPropDropScheme });
            //foreach (var prop in startProps)
            //{
            //    AddPropToInventory(inventory, prop);
            //}

            AddResource(inventory, "packed-food", 1);
            AddResource(inventory, "water-bottle", 1);
            AddResource(inventory, "med-kit", 1);

            return person;
        }

        private void FillSlot(HumanPerson person, IDropTableScheme dropScheme, int slotIndex)
        {
            // Генерируем предметы.
            // Выбираем предмет, как экипировку в слот.
            // Если он может быть экипирован, то устанавливаем в слот.
            // Остальные дропнутые предметы складываем просто в инвентарь.
            // Если текущий предмет невозможно экипировать, то его тоже помещаем в инвентарь.

            var inventory = person.Inventory;
            var dropedProps = _dropResolver.Resolve(new[] { dropScheme });
            var usedEquipment = dropedProps.OfType<Equipment>().FirstOrDefault();
            if (usedEquipment != null)
            {

                var canBeEquiped = CanBeEquiped(person.EquipmentCarrier, slotIndex, usedEquipment);
                if (canBeEquiped)
                {
                    AddEquipment(person.EquipmentCarrier, slotIndex, usedEquipment);
                    var unusedMainWeaponDrops = dropedProps.Where(x => x != usedEquipment).ToArray();
                    foreach (var prop in unusedMainWeaponDrops)
                    {
                        AddPropToInventory(inventory, prop);
                    }
                }
                else
                {
                    foreach (var prop in dropedProps)
                    {
                        AddPropToInventory(inventory, prop);
                    }
                }
            }
            else
            {
                foreach (var prop in dropedProps)
                {
                    AddPropToInventory(inventory, prop);
                }
            }
        }

        private static bool CanBeEquiped(
            IEquipmentCarrier equipmentCarrier,
            int slotIndex,
            Equipment equipment)
        {
            return EquipmentCarrierHelper.CanBeEquiped(equipmentCarrier, slotIndex, equipment);
        }

        private IDropTableScheme GetHeads()
        {
            return _schemeService.GetScheme<IDropTableScheme>(HEAD_DROP_SID);
        }

        private IDropTableScheme GetMainWeapons()
        {
            return _schemeService.GetScheme<IDropTableScheme>(MAIN_WEAPON_DROP_SID);
        }

        private IDropTableScheme GetArmors()
        {
            return _schemeService.GetScheme<IDropTableScheme>(BODY_DROP_SID);
        }

        private IDropTableScheme GetOffWeapons()
        {
            return _schemeService.GetScheme<IDropTableScheme>(OFF_WEAPON_DROP_SID);
        }

        private IDropTableScheme GetStartProps()
        {
            return _schemeService.GetScheme<IDropTableScheme>(START_PROP_DROP_SID);
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
