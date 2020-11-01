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
        private readonly IDice _dice;
        private readonly IDropResolver _dropResolver;
        private readonly IPersonPerkInitializator _personPerkInitializator;
        private readonly IPropFactory _propFactory;

        private readonly ISchemeService _schemeService;
        private readonly ISurvivalRandomSource _survivalRandomSource;

        public RandomHumanPersonFactory(
            ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            IDropResolver dropResolver,
            IPersonPerkInitializator personPerkInitializator,
            IDice dice)
        {
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _survivalRandomSource =
                survivalRandomSource ?? throw new ArgumentNullException(nameof(survivalRandomSource));
            _propFactory = propFactory ?? throw new ArgumentNullException(nameof(propFactory));
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _personPerkInitializator = personPerkInitializator ??
                                       throw new ArgumentNullException(nameof(personPerkInitializator));
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        }

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        public IPerson Create(string personSchemeSid, IFraction fraction)
        {
            IPersonScheme personScheme = _schemeService.GetScheme<IPersonScheme>(personSchemeSid);

            HumanPerson person = new HumanPerson(personScheme, fraction);

            IAttributesModule attributeModule = RollAndAddPersonAttributesToPerson(person);

            MovingModule movingModule = new MovingModule(attributeModule);
            person.AddModule(movingModule);

            InventoryModule inventoryModule = new InventoryModule();
            person.AddModule(inventoryModule);

            EquipmentModule equipmentModule = new EquipmentModule(personScheme.Slots);
            person.AddModule(equipmentModule);

            EffectsModule effectsModule = new EffectsModule();
            person.AddModule(effectsModule);

            EvolutionModule evolutionModule = new EvolutionModule(_schemeService);
            person.AddModule(evolutionModule);
            RollTraitPerks(evolutionModule);

            HumanSurvivalModule survivalModule =
                new HumanSurvivalModule(personScheme, _survivalRandomSource, attributeModule, effectsModule,
                    evolutionModule, equipmentModule) {PlayerEventLogService = PlayerEventLogService};
            person.AddModule(survivalModule);

            RollStartEquipment(inventoryModule, person);

            ITacticalActScheme defaultActScheme =
                _schemeService.GetScheme<ITacticalActScheme>(person.Scheme.DefaultAct);
            CombatActModule combatActModule =
                new CombatActModule(defaultActScheme, equipmentModule, effectsModule, evolutionModule);
            person.AddModule(combatActModule);

            CombatStatsModule combatStatsModule = new CombatStatsModule(evolutionModule, equipmentModule);
            person.AddModule(combatStatsModule);

            DiseaseModule diseaseModule = new DiseaseModule();
            person.AddModule(diseaseModule);

            FowData fowModule = new FowData();
            person.AddModule(fowModule);

            person.PlayerEventLogService = PlayerEventLogService;

            return person;
        }

        private IAttributesModule RollAndAddPersonAttributesToPerson(IPerson person)
        {
            PersonAttribute[] attributes = new[]
            {
                RollAttribute(PersonAttributeType.PhysicalStrength), RollAttribute(PersonAttributeType.Dexterity),
                RollAttribute(PersonAttributeType.Perception), RollAttribute(PersonAttributeType.Constitution)
            };

            AttributesModule attributesModule = new AttributesModule(attributes);

            person.AddModule(attributesModule);

            return attributesModule;
        }

        private PersonAttribute RollAttribute(PersonAttributeType attributeType)
        {
            var value = 10 + _dice.Roll(-4, 4);
            return new PersonAttribute(attributeType, value);
        }

        private void RollTraitPerks(IEvolutionModule evolutionData)
        {
            if (evolutionData is null)
            {
                throw new ArgumentNullException(nameof(evolutionData));
            }

            IPerk[] rolledTraits = _personPerkInitializator.Generate();
            evolutionData.AddBuildInPerks(rolledTraits);
        }

        private void RollStartEquipment(IInventoryModule inventory, HumanPerson person)
        {
            IDropTableScheme headDropScheme = GetHeads();
            FillSlot(person, headDropScheme, HEAD_SLOT_INDEX);

            IDropTableScheme armorDropScheme = GetArmors();
            FillSlot(person, armorDropScheme, BODY_SLOT_INDEX);

            IDropTableScheme mainWeaponDropScheme = GetMainWeapons();
            FillSlot(person, mainWeaponDropScheme, MAIN_HAND_SLOT_INDEX);

            IDropTableScheme offWeaponDropScheme = GetOffWeapons();
            FillSlot(person, offWeaponDropScheme, OFF_HAND_SLOT_INDEX);

            IDropTableScheme startPropDropScheme = GetStartProps();
            IProp[] startProps = _dropResolver.Resolve(new[] {startPropDropScheme});
            foreach (IProp prop in startProps)
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
            IPropScheme scheme = _schemeService.GetScheme<IPropScheme>(sid);
            Equipment prop = _propFactory.CreateEquipment(scheme);
            AddPropToInventory(inventory, prop);
        }

        private void FillSlot(HumanPerson person, IDropTableScheme dropScheme, int slotIndex)
        {
            // Генерируем предметы.
            // Выбираем предмет, как экипировку в слот.
            // Если он может быть экипирован, то устанавливаем в слот.
            // Остальные дропнутые предметы складываем просто в инвентарь.
            // Если текущий предмет невозможно экипировать, то его тоже помещаем в инвентарь.

            IInventoryModule inventory = person.GetModule<IInventoryModule>();
            IProp[] dropedProps = _dropResolver.Resolve(new[] {dropScheme});
            var usedEquipment = dropedProps.OfType<Equipment>().FirstOrDefault();
            if (usedEquipment != null)
            {
                IEquipmentModule equipmentModule = person.GetModule<IEquipmentModule>();
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
                    foreach (IProp prop in dropedProps)
                    {
                        AddPropToInventory(inventory, prop);
                    }
                }
            }
            else
            {
                foreach (IProp prop in dropedProps)
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

        private static void AddPropToInventory(IPropStore inventory, IProp prop)
        {
            inventory.Add(prop);
        }

        private void AddResource(IInventoryModule inventory, string resourceSid, int count)
        {
            try
            {
                IPropScheme resourceScheme = _schemeService.GetScheme<IPropScheme>(resourceSid);
                Resource resource = _propFactory.CreateResource(resourceScheme, count);
                inventory.Add(resource);
            }
            catch (KeyNotFoundException exception)
            {
                throw new CreatePersonException($"Не найден объект {resourceSid}", exception);
            }
        }
    }
}