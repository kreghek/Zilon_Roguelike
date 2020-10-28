using System;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonGeneration
{
    public sealed class DemigodPersonFactory : IPersonFactory
    {
        private readonly ISchemeService _schemeService;
        private readonly ISurvivalRandomSource _survivalRandomSource;
        private readonly IPersonPerkInitializator _personPerkInitializator;
        private readonly IDice _dice;

        public DemigodPersonFactory(
            ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPersonPerkInitializator personPerkInitializator,
            IDice dice)
        {
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _survivalRandomSource = survivalRandomSource ?? throw new ArgumentNullException(nameof(survivalRandomSource));
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

            var survivalModule = new HumanSurvivalModule(
                personScheme,
                _survivalRandomSource,
                attributeModule,
                effectsModule,
                evolutionModule,
                equipmentModule);
            person.AddModule(survivalModule);

            var defaultActScheme = _schemeService.GetScheme<ITacticalActScheme>(person.Scheme.DefaultAct);
            var combatActModule = new CombatActModule(defaultActScheme, equipmentModule, effectsModule, evolutionModule);
            person.AddModule(combatActModule);

            var combatStatsModule = new CombatStatsModule(evolutionModule, equipmentModule);
            person.AddModule(combatStatsModule);

            var diseaseModule = new DiseaseModule();
            person.AddModule(diseaseModule);

            var fowModule = new FowData();
            person.AddModule(fowModule);

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

            var rolledTraits = _personPerkInitializator.Get();
            evolutionData.AddBuildInPerks(rolledTraits);
        }
    }
}