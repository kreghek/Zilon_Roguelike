using Zilon.Core.CommonServices.Dices;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Zilon.Core.PersonGeneration
{
    public abstract class FullPersonFactoryBase : IPersonFactory
    {
        private readonly IPersonPerkInitializator _personPerkInitializator;
        private readonly IPropFactory _propFactory;
        private readonly ISurvivalRandomSource _survivalRandomSource;

        protected FullPersonFactoryBase(
            ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            IDropResolver dropResolver,
            IPersonPerkInitializator personPerkInitializator,
            IDice dice)
        {
            SchemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _survivalRandomSource =
                survivalRandomSource ?? throw new ArgumentNullException(nameof(survivalRandomSource));
            _propFactory = propFactory ?? throw new ArgumentNullException(nameof(propFactory));
            DropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _personPerkInitializator = personPerkInitializator ??
                                       throw new ArgumentNullException(nameof(personPerkInitializator));
            Dice = dice ?? throw new ArgumentNullException(nameof(dice));
        }

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        protected IDice Dice { get; }

        protected IDropResolver DropResolver { get; }

        protected ISchemeService SchemeService { get; }

        protected static int HeadSlotIndex => 0;

        protected static int BodySlotIndex => 1;

        protected static int MainHandSlotIndex => 2;

        protected static int OffHandSlotIndex => 3;

        public IPerson Create(string personSchemeSid, IFraction fraction)
        {
            IPersonScheme personScheme = SchemeService.GetScheme<IPersonScheme>(personSchemeSid);

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

            EvolutionModule evolutionModule = new EvolutionModule(SchemeService);
            person.AddModule(evolutionModule);
            RollTraitPerks(evolutionModule);

            HumanSurvivalModule survivalModule =
                new HumanSurvivalModule(personScheme, _survivalRandomSource, attributeModule, effectsModule,
                    evolutionModule, equipmentModule) {PlayerEventLogService = PlayerEventLogService};
            person.AddModule(survivalModule);

            RollStartEquipment(inventoryModule, person);

            ITacticalActScheme defaultActScheme = SchemeService.GetScheme<ITacticalActScheme>(person.Scheme.DefaultAct);
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
            var value = 10 + Dice.Roll(-4, 4);
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

        protected abstract void RollStartEquipment(IInventoryModule inventory, HumanPerson person);

        protected void FillSlot(HumanPerson person, IDropTableScheme dropScheme, int slotIndex)
        {
            // Генерируем предметы.
            // Выбираем предмет, как экипировку в слот.
            // Если он может быть экипирован, то устанавливаем в слот.
            // Остальные дропнутые предметы складываем просто в инвентарь.
            // Если текущий предмет невозможно экипировать, то его тоже помещаем в инвентарь.

            IInventoryModule inventory = person.GetModule<IInventoryModule>();
            IProp[] dropedProps = DropResolver.Resolve(new[] {dropScheme});
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

        protected static void AddEquipment(IEquipmentModule equipmentModule, int slotIndex, Equipment equipment)
        {
            if (equipmentModule is null)
            {
                throw new ArgumentNullException(nameof(equipmentModule));
            }

            equipmentModule[slotIndex] = equipment;
        }

        protected static void AddPropToInventory(IPropStore inventory, IProp prop)
        {
            if (inventory is null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            inventory.Add(prop);
        }

        protected void AddResource(IInventoryModule inventory, string resourceSid, int count)
        {
            if (inventory is null)
            {
                throw new ArgumentNullException(nameof(inventory));
            }

            try
            {
                IPropScheme resourceScheme = SchemeService.GetScheme<IPropScheme>(resourceSid);
                Resource resource = _propFactory.CreateResource(resourceScheme, count);
                inventory.Add(resource);
            }
            catch (KeyNotFoundException exception)
            {
                throw new CreatePersonException($"Не найден объект {resourceSid}", exception);
            }
        }

        protected void AddEquipment(IInventoryModule inventory, string sid)
        {
            IPropScheme scheme = SchemeService.GetScheme<IPropScheme>(sid);
            Equipment prop = _propFactory.CreateEquipment(scheme);
            AddPropToInventory(inventory, prop);
        }

        protected void AddDefaultProps(IInventoryModule inventory)
        {
            AddResource(inventory, "packed-food", 1);
            AddResource(inventory, "water-bottle", 1);
            AddResource(inventory, "med-kit", 1);
        }
    }
}