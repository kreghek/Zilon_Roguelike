using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Zilon.Core.PersonGeneration
{
    public sealed class RandomHumanPersonFactory : IPersonFactory
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
        private readonly IPersonPerkInitializator _personPerkInitializator;
        private readonly IDice _dice;

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        public RandomHumanPersonFactory(
            ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            IDropResolver dropResolver,
            IPersonPerkInitializator personPerkInitializator,
            IDice dice)
        {
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _survivalRandomSource = survivalRandomSource ?? throw new ArgumentNullException(nameof(survivalRandomSource));
            _propFactory = propFactory ?? throw new ArgumentNullException(nameof(propFactory));
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _personPerkInitializator = personPerkInitializator ?? throw new ArgumentNullException(nameof(personPerkInitializator));
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        }

        public IPerson Create(string personSchemeSid)
        {
            var personScheme = _schemeService.GetScheme<IPersonScheme>(personSchemeSid);

            var person = new HumanPerson(personScheme);

            var inventoryModule = new InventoryModule();
            person.AddModule(inventoryModule);

            var equipmentModule = new EquipmentModule(personScheme.Slots);
            person.AddModule(equipmentModule);

            var effectsModule = new EffectsModule();
            person.AddModule(effectsModule);

            var evolutionModule = new EvolutionModule(_schemeService);
            person.AddModule(evolutionModule);
            RollTraitPerks(evolutionModule);

            var survivalModule = new HumanSurvivalModule(personScheme, _survivalRandomSource, effectsModule, evolutionModule, equipmentModule)
            {
                PlayerEventLogService = PlayerEventLogService
            };
            person.AddModule(survivalModule);

            RollAndAddPersonAttributesToPerson(person);

            RollStartEquipment(inventoryModule, person);

            var defaultActScheme = _schemeService.GetScheme<ITacticalActScheme>(person.Scheme.DefaultAct);
            var combatActModule = new CombatActModule(defaultActScheme, equipmentModule, effectsModule, evolutionModule);
            person.AddModule(combatActModule);

            var combatStatsModule = new CombatStatsModule(evolutionModule, equipmentModule);
            person.AddModule(combatStatsModule);

            var diseaseModule = new DiseaseModule();
            person.AddModule(diseaseModule);

            person.PlayerEventLogService = PlayerEventLogService;

            return person;
        }

        private void RollAndAddPersonAttributesToPerson(IPerson person)
        {
            var attributes = new[] {
                RollAttribute(PersonAttributeType.PhysicalStrength),
                RollAttribute(PersonAttributeType.Dexterity),
                RollAttribute(PersonAttributeType.Perception),
                RollAttribute(PersonAttributeType.Constitution)
            };

            var attributesModule = new AttributesModule(attributes);

            person.AddModule(attributesModule);
        }

        private PersonAttribute RollAttribute(PersonAttributeType attributeType)
        {
            var value = 10 + _dice.Roll(-5, 5);
            return new PersonAttribute(attributeType, value);
        }

        private void RollTraitPerks(IEvolutionModule evolutionData)
        {
            if (evolutionData is null)
            {
                throw new ArgumentNullException(nameof(evolutionData));
            }

            var rolledTraits = _personPerkInitializator.Get();
            evolutionData.AddBuildInPerks(rolledTraits);
        }

        private void RollStartEquipment(IInventoryModule inventory, HumanPerson person)
        {
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
            AddEquipment(inventory, "pick-axe");
            AddEquipment(inventory, "shovel");
            AddEquipment(inventory, "shotgun");
            AddResource(inventory, "bullet-45", 100);
        }

        private void AddEquipment(IInventoryModule inventory, string sid)
        {
            var scheme = _schemeService.GetScheme<IPropScheme>(sid);
            var prop = _propFactory.CreateEquipment(scheme);
            AddPropToInventory(inventory, prop);
        }

        private void FillSlot(HumanPerson person, IDropTableScheme dropScheme, int slotIndex)
        {
            // Генерируем предметы.
            // Выбираем предмет, как экипировку в слот.
            // Если он может быть экипирован, то устанавливаем в слот.
            // Остальные дропнутые предметы складываем просто в инвентарь.
            // Если текущий предмет невозможно экипировать, то его тоже помещаем в инвентарь.

            var inventory = person.GetModule<IInventoryModule>();
            var dropedProps = _dropResolver.Resolve(new[] { dropScheme });
            var usedEquipment = dropedProps.OfType<Equipment>().FirstOrDefault();
            if (usedEquipment != null)
            {
                var equipmentModule = person.GetModule<IEquipmentModule>();
                var canBeEquiped = CanBeEquiped(equipmentModule, slotIndex, usedEquipment);
                if (canBeEquiped)
                {
                    AddEquipment(equipmentModule, slotIndex, usedEquipment);
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
            IEquipmentModule equipmentModule,
            int slotIndex,
            Equipment equipment)
        {
            return EquipmentCarrierHelper.CanBeEquiped(equipmentModule, slotIndex, equipment);
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

        private static void AddEquipment(IEquipmentModule equipmentModule, int slotIndex, Equipment equipment)
        {
            equipmentModule[slotIndex] = equipment;
        }

        private void AddPropToInventory(IPropStore inventory, IProp prop)
        {
            inventory.Add(prop);
        }

        private void AddResource(IInventoryModule inventory, string resourceSid, int count)
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
