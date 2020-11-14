using System;
using Zilon.Core.PersonGeneration;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

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

        public IPerson Create(string personSchemeSid, IFraction fraction)
        {
            IPersonScheme personScheme = _schemeService.GetScheme<IPersonScheme>(personSchemeSid);

            HumanPerson person = new HumanPerson(personScheme, fraction);

            PersonAttribute[] attributes =
            {
                new PersonAttribute(PersonAttributeType.PhysicalStrength, 10),
                new PersonAttribute(PersonAttributeType.Dexterity, 10),
                new PersonAttribute(PersonAttributeType.Perception, 10),
                new PersonAttribute(PersonAttributeType.Constitution, 10)
            };
            AttributesModule attributesModule = new AttributesModule(attributes);
            person.AddModule(attributesModule);

            InventoryModule inventoryModule = new InventoryModule();
            person.AddModule(inventoryModule);

            EquipmentModule equipmentModule = new EquipmentModule(personScheme.Slots);
            person.AddModule(equipmentModule);

            EffectsModule effectsModule = new EffectsModule();
            person.AddModule(effectsModule);

            EvolutionModule evolutionModule = new EvolutionModule(_schemeService);
            person.AddModule(evolutionModule);

            HumanSurvivalModule survivalModule = new HumanSurvivalModule(personScheme, _survivalRandomSource,
                attributesModule, effectsModule, evolutionModule, equipmentModule);
            person.AddModule(survivalModule);

            ITacticalActScheme defaultActScheme =
                _schemeService.GetScheme<ITacticalActScheme>(person.Scheme.DefaultAct);
            CombatActModule combatActModule =
                new CombatActModule(defaultActScheme, equipmentModule, effectsModule, evolutionModule);
            person.AddModule(combatActModule);

            CombatStatsModule combatStatsModule = new CombatStatsModule(evolutionModule, equipmentModule);
            person.AddModule(combatStatsModule);

            DiseaseModule diseaseModule = new DiseaseModule();
            person.AddModule(diseaseModule);

            return person;
        }
    }
}