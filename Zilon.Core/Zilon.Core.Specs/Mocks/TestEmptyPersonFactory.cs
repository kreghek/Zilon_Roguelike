using System;

using Zilon.Core.PersonGeneration;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;

namespace Zilon.Core.Specs.Mocks
{
    public sealed class TestEmptyPersonFactory : IPersonFactory
    {
        private readonly ISchemeService _schemeService;
        private readonly ISurvivalRandomSource _survivalRandomSource;

        public TestEmptyPersonFactory(ISchemeService schemeService, ISurvivalRandomSource survivalRandomSource)
        {
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _survivalRandomSource =
                survivalRandomSource ?? throw new ArgumentNullException(nameof(survivalRandomSource));
        }

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        public IPerson Create(string personSchemeSid, IFraction fraction)
        {
            var personScheme = _schemeService.GetScheme<IPersonScheme>(personSchemeSid);

            var person = new HumanPerson(personScheme, fraction);

            var attributes = new[]
            {
                new PersonAttribute(PersonAttributeType.PhysicalStrength, 10),
                new PersonAttribute(PersonAttributeType.Dexterity, 10),
                new PersonAttribute(PersonAttributeType.Perception, 10),
                new PersonAttribute(PersonAttributeType.Constitution, 10)
            };
            var attributesModule = new AttributesModule(attributes);
            person.AddModule(attributesModule);

            var inventoryModule = new InventoryModule();
            person.AddModule(inventoryModule);

            var equipmentModule = new EquipmentModule(personScheme.Slots);
            person.AddModule(equipmentModule);

            var сonditionModule = new ConditionsModule();
            person.AddModule(сonditionModule);

            var evolutionModule = new EvolutionModule(_schemeService);
            person.AddModule(evolutionModule);

            var survivalModule = new HumanSurvivalModule(personScheme, _survivalRandomSource, attributesModule,
                сonditionModule, evolutionModule, equipmentModule);
            person.AddModule(survivalModule);

            var defaultActScheme = _schemeService.GetScheme<ITacticalActScheme>(person.Scheme.DefaultActs);
            var combatActModule =
                new CombatActModule(defaultActScheme, equipmentModule, сonditionModule, evolutionModule);
            person.AddModule(combatActModule);

            var combatStatsModule = new CombatStatsModule(evolutionModule, equipmentModule);
            person.AddModule(combatStatsModule);

            var diseaseModule = new DiseaseModule(сonditionModule);
            person.AddModule(diseaseModule);

            return person;
        }
    }
}