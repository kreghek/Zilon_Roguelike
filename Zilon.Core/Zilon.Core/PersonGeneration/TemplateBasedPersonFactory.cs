using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Localization;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Zilon.Core.PersonGeneration
{
    public sealed class TemplateBasedPersonFactory : IPersonFactory
    {
        private const int HEAD_SLOT_INDEX = 0;
        private const int BODY_SLOT_INDEX = 1;
        private const int MAIN_HAND_SLOT_INDEX = 2;
        private const int OFF_HAND_SLOT_INDEX = 3;

        private readonly ISchemeService _schemeService;
        private readonly ISurvivalRandomSource _survivalRandomSource;
        private readonly IPropFactory _propFactory;
        private readonly IDropResolver _dropResolver;
        private readonly IPersonPerkInitializator _personPerkInitializator;
        private readonly IDice _dice;

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        public TemplateBasedPersonFactory(
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

        public IPerson Create(string personSchemeSid, IFraction fraction)
        {
            var personScheme = _schemeService.GetScheme<IPersonScheme>(personSchemeSid);

            var person = new HumanPerson(personScheme, fraction);

            var attributeModule = RollAndAddPersonAttributesToPerson(person);

            var movingModule = new MovingModule(attributeModule);
            person.AddModule(movingModule);

            var inventoryModule = new InventoryModule();
            person.AddModule(inventoryModule);

            var equipmentModule = new EquipmentModule(personScheme.Slots);
            person.AddModule(equipmentModule);

            var effectsModule = new EffectsModule();
            person.AddModule(effectsModule);

            var evolutionModule = new EvolutionModule(_schemeService);
            person.AddModule(evolutionModule);
            RollTraitPerks(evolutionModule);

            var survivalModule = new HumanSurvivalModule(personScheme, _survivalRandomSource, attributeModule, effectsModule, evolutionModule, equipmentModule)
            {
                PlayerEventLogService = PlayerEventLogService
            };
            person.AddModule(survivalModule);

            RollStartEquipment(inventoryModule, person);

            var defaultActScheme = _schemeService.GetScheme<ITacticalActScheme>(person.Scheme.DefaultAct);
            var combatActModule = new CombatActModule(defaultActScheme, equipmentModule, effectsModule, evolutionModule);
            person.AddModule(combatActModule);

            var combatStatsModule = new CombatStatsModule(evolutionModule, equipmentModule);
            person.AddModule(combatStatsModule);

            var diseaseModule = new DiseaseModule();
            person.AddModule(diseaseModule);

            var fowModule = new FowData();
            person.AddModule(fowModule);

            person.PlayerEventLogService = PlayerEventLogService;

            return person;
        }

        private IAttributesModule RollAndAddPersonAttributesToPerson(IPerson person)
        {
            var attributes = new[] {
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
            var value = 10 + _dice.Roll(-4, 4);
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

        private void RollStartEquipment(IInventoryModule inventory, HumanPerson person)
        {
            var templates = GetPersonTemplateByFraction(person.Fraction);
            var rolledTemplate = _dice.RollFromList(templates);

            person.PersonEquipmentTemplate = rolledTemplate.Name;

            var headDropScheme = rolledTemplate.HeadEquipments;
            FillSlot(person, headDropScheme, HEAD_SLOT_INDEX);

            var armorDropScheme = rolledTemplate.BodyEquipments;
            FillSlot(person, armorDropScheme, BODY_SLOT_INDEX);

            var mainWeaponDropScheme = rolledTemplate.MainHandEquipments;
            FillSlot(person, mainWeaponDropScheme, MAIN_HAND_SLOT_INDEX);

            var offWeaponDropScheme = rolledTemplate.OffHandEquipments;
            FillSlot(person, offWeaponDropScheme, OFF_HAND_SLOT_INDEX);

            var startPropDropScheme = rolledTemplate.InventoryProps;
            var startProps = _dropResolver.Resolve(new[] { startPropDropScheme });
            foreach (var prop in startProps)
            {
                AddPropToInventory(inventory, prop);
            }

            AddResource(inventory, "packed-food", 1);
            AddResource(inventory, "water-bottle", 1);
            AddResource(inventory, "med-kit", 1);
        }

        private PersonTemplate[] GetPersonTemplateByFraction(IFraction fraction)
        {
            if (fraction == Fractions.InterventionistFraction)
            {
                return GetInterventionalistsPersonTemplates();
            }
            else if (fraction == Fractions.MilitiaFraction)
            {
                return GetMilitiaPersonTemplates();
            }
            else if (fraction == Fractions.TroublemakerFraction)
            {
                return GetTroublemakerPersonTemplates();
            }
            else
            {
                return GetPersonTemplates();
            }
        }

        private sealed class PersonTemplate
        {
            public ILocalizedString Name { get; set; }
            public IDropTableScheme HeadEquipments { get; set; }
            public IDropTableScheme BodyEquipments { get; set; }
            public IDropTableScheme MainHandEquipments { get; set; }
            public IDropTableScheme OffHandEquipments { get; set; }
            public IDropTableScheme InventoryProps { get; set; }
        }

        private sealed class StartDropTableScheme : IDropTableScheme
        {
            public StartDropTableScheme()
            {
                Records = Array.Empty<IDropTableRecordSubScheme>();
            }

            public IDropTableRecordSubScheme[] Records { get; set; }
            public int Rolls { get => 1; }
            public string Sid { get; set; }
            public bool Disabled { get; }
            public LocalizedStringSubScheme Name { get; }
            public LocalizedStringSubScheme Description { get; }
        }

        private sealed class StartDropTableRecordSubScheme : IDropTableRecordSubScheme
        {
            public StartDropTableRecordSubScheme()
            {
                Weight = 1;
            }

            public int MaxCount { get; set; }
            public int MinCount { get; set; }
            public string SchemeSid { get; set; }
            public int Weight { get; set; }
            public IDropTableScheme[] Extra { get; set; }
        }

        private static PersonTemplate[] GetInterventionalistsPersonTemplates()
        {
            return new[] {
                new PersonTemplate{
                    Name = new LocalizedString{ Ru = "Легкий интервент", En = "Light Interventionalist" },
                    BodyEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="quilted-coat", Weight = 1 }
                        }
                    },
                    HeadEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="gas-mask", Weight = 1 }
                        }
                    },
                    MainHandEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="rough-sword", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="tribal-spear", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="knight-sword", Weight = 1 },
                        }
                    },
                },
                new PersonTemplate{
                    Name = new LocalizedString{ Ru = "Тяжелый интервент", En = "Heavy Interventionalist" },
                    BodyEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="steel-armor", Weight = 1 }
                        }
                    },
                    HeadEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="gas-mask", Weight = 1 }
                        }
                    },
                    MainHandEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="rough-sword", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="tribal-spear", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="knight-sword", Weight = 1 },
                        }
                    },
                }
            };
        }

        private static PersonTemplate[] GetTroublemakerPersonTemplates()
        {
            return new[] {
                new PersonTemplate{
                    Name = new LocalizedString{ Ru = "Смутьян", En = "Troblemaker" },
                    BodyEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="quilted-coat", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="scholar-robe", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="leather-jacket", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="master-robe", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 1 },
                        }
                    },
                    HeadEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="gas-mask", Weight = 1 }
                        }
                    },
                    MainHandEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="rough-sword", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="tribal-spear", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="knight-sword", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="short-sword", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="battle-axe", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="mace", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="club", Weight = 1 },
                        }
                    },
                    OffHandEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="wooden-shield", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="short-sword", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="club", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="mace", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 1 },
                        }
                    },
                }
            };
        }

        private static PersonTemplate[] GetMilitiaPersonTemplates()
        {
            return new[] {
                new PersonTemplate{
                    Name = new LocalizedString{ Ru = "Легкий интервент", En = "Light Interventionalist" },
                    BodyEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="quilted-coat", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="scholar-robe", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="leather-jacket", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="master-robe", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 1 },
                        }
                    },
                    HeadEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="tactical-helmet", Weight = 1 }
                        }
                    },
                    MainHandEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="short-sword", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="battle-axe", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="club", Weight = 1 },
                        }
                    },
                    OffHandEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="wooden-shield", Weight = 2 },
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 1 },
                        }
                    },
                }
            };
        }

        private static PersonTemplate[] GetPersonTemplates()
        {
            return new[] {
                new PersonTemplate{
                    Name = new LocalizedString{ Ru = "Горец", En = "Hightlander" },
                    BodyEquipments = new StartDropTableScheme(),
                    HeadEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="highlander-helmet", Weight = 1 }
                        }
                    },
                    MainHandEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="short-sword", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="battle-axe", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="claw-sword", Weight = 1 },
                        }
                    },
                    OffHandEquipments = new StartDropTableScheme(),
                    InventoryProps = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="med-kit", MinCount = 1, MaxCount=1 , Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="packed-food", MinCount = 1, MaxCount=1, Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="water-bottle", MinCount = 1, MaxCount=1, Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 4 },
                        }
                    }
                },
                new PersonTemplate{
                    Name = new LocalizedString{ Ru = "Самурай", En = "Samurai" },
                    BodyEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="leather-jacket", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 1 },
                        }
                    },
                    HeadEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 1 }
                        }
                    },
                    MainHandEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="bocken", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="dikatan", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="katana", Weight = 1 },
                        }
                    },
                    OffHandEquipments = new StartDropTableScheme(),
                    InventoryProps = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="med-kit", MinCount = 1, MaxCount=1 , Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="packed-food", MinCount = 1, MaxCount=1, Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="water-bottle", MinCount = 1, MaxCount=1, Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 4 },
                        }
                    }
                },
                new PersonTemplate{
                    Name = new LocalizedString{ Ru = "Гвардеец", En = "Guardian" },
                    BodyEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="steel-armor", Weight = 2 },
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 1 },
                        }
                    },
                    HeadEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 1 }
                        }
                    },
                    MainHandEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="short-sword", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid="glaive", Weight = 1 },
                        }
                    },
                    OffHandEquipments = new StartDropTableScheme{
                        Records = new IDropTableRecordSubScheme[]{
                            new StartDropTableRecordSubScheme{ SchemeSid="wooden-shield", Weight = 1 },
                            new StartDropTableRecordSubScheme{ SchemeSid=null, Weight = 2 },
                        }
                    },
                    InventoryProps = new StartDropTableScheme()
                }
            };
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