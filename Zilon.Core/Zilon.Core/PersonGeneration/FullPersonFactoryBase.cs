using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public abstract class FullPersonFactoryBase : IPersonFactory
    {
        private readonly IPersonPerkInitializator _personPerkInitializator;
        private readonly IPropFactory _propFactory;
        private readonly ISurvivalRandomSource _survivalRandomSource;

        [ExcludeFromCodeCoverage]
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

        protected static int BodySlotIndex => 1;

        protected IDice Dice { get; }

        protected IDropResolver DropResolver { get; }

        protected static int HeadSlotIndex => 0;

        protected static int MainHandSlotIndex => 2;

        protected static int OffHandSlotIndex => 3;

        protected ISchemeService SchemeService { get; }

        protected void AddDefaultProps(IInventoryModule inventory)
        {
            AddResource(inventory, "packed-food", 1);
            AddResource(inventory, "water-bottle", 1);
            AddResource(inventory, "med-kit", 1);
            AddEquipment(inventory, "med-mask");
            AddEquipment(inventory, "med-gloves");
        }

        protected static void AddEquipment(IEquipmentModule equipmentModule, int slotIndex, Equipment equipment)
        {
            if (equipmentModule is null)
            {
                throw new ArgumentNullException(nameof(equipmentModule));
            }

            equipmentModule[slotIndex] = equipment;
        }

        protected void AddEquipment(IInventoryModule inventory, string sid)
        {
            var scheme = SchemeService.GetScheme<IPropScheme>(sid);
            var prop = _propFactory.CreateEquipment(scheme);
            AddPropToInventory(inventory, prop);
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
                var resourceScheme = SchemeService.GetScheme<IPropScheme>(resourceSid);
                var resource = _propFactory.CreateResource(resourceScheme, count);
                inventory.Add(resource);
            }
            catch (KeyNotFoundException exception)
            {
                throw new CreatePersonException($"Не найден объект {resourceSid}", exception);
            }
        }

        protected void FillSlot(HumanPerson person, IDropTableScheme? dropScheme, int slotIndex)
        {
            if (dropScheme is null)
            {
                return;
            }

            // Генерируем предметы.
            // Выбираем предмет, как экипировку в слот.
            // Если он может быть экипирован, то устанавливаем в слот.
            // Остальные дропнутые предметы складываем просто в инвентарь.
            // Если текущий предмет невозможно экипировать, то его тоже помещаем в инвентарь.

            var inventory = person.GetModule<IInventoryModule>();
            var dropedProps = DropResolver.Resolve(new[] { dropScheme });
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

        protected abstract void RollStartEquipment(IInventoryModule inventory, HumanPerson person);

        private static bool CanBeEquiped(
            IEquipmentModule equipmentModule,
            int slotIndex,
            Equipment equipment)
        {
            return EquipmentCarrierHelper.CanBeEquiped(equipmentModule, slotIndex, equipment);
        }

        private IAttributesModule RollAndAddPersonAttributesToPerson(IPerson person)
        {
            var attributes = new[]
            {
                RollAttribute(PersonAttributeType.PhysicalStrength),
                RollAttribute(PersonAttributeType.Dexterity),
                RollAttribute(PersonAttributeType.Perception),
                RollAttribute(PersonAttributeType.Constitution)
            };

            var attributesModule = new AttributesModule(attributes);

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

            var rolledTraits = _personPerkInitializator.Generate();
            evolutionData.AddBuildInPerks(rolledTraits);
        }

        public IPlayerEventLogService? PlayerEventLogService { get; set; }

        public IPerson Create(string personSchemeSid, IFraction fraction)
        {
            var personScheme = SchemeService.GetScheme<IPersonScheme>(personSchemeSid);

            var person = new HumanPerson(personScheme, fraction);

            var attributeModule = RollAndAddPersonAttributesToPerson(person);

            var movingModule = new MovingModule(attributeModule);
            person.AddModule(movingModule);

            var inventoryModule = new InventoryModule();
            person.AddModule(inventoryModule);

            var notNullSlots = personScheme.Slots.Select(x => x!).ToArray();
            var equipmentModule = new EquipmentModule(notNullSlots);
            person.AddModule(equipmentModule);

            var effectsModule = new ConditionsModule();
            person.AddModule(effectsModule);

            var evolutionModule = new EvolutionModule(SchemeService);
            person.AddModule(evolutionModule);
            RollTraitPerks(evolutionModule);

            var survivalModule = new HumanSurvivalModule(personScheme, _survivalRandomSource, attributeModule,
                effectsModule, evolutionModule, equipmentModule)
            {
                PlayerEventLogService = PlayerEventLogService
            };
            person.AddModule(survivalModule);

            RollStartEquipment(inventoryModule, person);

            if (person.Scheme.DefaultActs is null)
            {
                throw new InvalidOperationException();
            }

            var defaultActSchemes =
                person.Scheme.DefaultActs.Select(x => SchemeService.GetScheme<ITacticalActScheme>(x));
            var combatActModule = new CombatActModule(
                defaultActSchemes,
                equipmentModule,
                effectsModule,
                evolutionModule);

            person.AddModule(combatActModule);

            var combatStatsModule = new CombatStatsModule(evolutionModule, equipmentModule);
            person.AddModule(combatStatsModule);

            var diseaseModule = new DiseaseModule(effectsModule);
            person.AddModule(diseaseModule);

            var fowModule = new FowData();
            person.AddModule(fowModule);

            person.PlayerEventLogService = PlayerEventLogService;

            return person;
        }
    }
}