using System;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Localization;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.PersonGeneration
{
    public sealed class TemplateBasedPersonFactory : FullPersonFactoryBase
    {
        public TemplateBasedPersonFactory(
            ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            IDropResolver dropResolver,
            IPersonPerkInitializator personPerkInitializator,
            IDice dice) : base(schemeService, survivalRandomSource, propFactory, dropResolver, personPerkInitializator, dice)
        {
        }

        protected override void RollStartEquipment(IInventoryModule inventory, HumanPerson person)
        {
            var templates = GetPersonTemplateByFraction(person.Fraction, SchemeService);
            var rolledTemplate = Dice.RollFromList(templates);

            person.PersonEquipmentTemplate = rolledTemplate.Name;

            var headDropScheme = rolledTemplate.HeadEquipments;
            FillSlot(person, headDropScheme, HeadSlotIndex);

            var armorDropScheme = rolledTemplate.BodyEquipments;
            FillSlot(person, armorDropScheme, BodySlotIndex);

            var mainWeaponDropScheme = rolledTemplate.MainHandEquipments;
            FillSlot(person, mainWeaponDropScheme, MainHandSlotIndex);

            var offWeaponDropScheme = rolledTemplate.OffHandEquipments;
            FillSlot(person, offWeaponDropScheme, OffHandSlotIndex);

            var startPropDropScheme = rolledTemplate.InventoryProps;
            var startProps = DropResolver.Resolve(new[] { startPropDropScheme });
            foreach (var prop in startProps)
            {
                AddPropToInventory(inventory, prop);
            }

            AddDefaultProps(inventory);
        }

        private static IPersonTemplateScheme[] GetPersonTemplateByFraction(IFraction fraction, ISchemeService schemeService)
        {
            if (fraction == Fractions.InterventionistFraction)
            {
                return GetInterventionalistsPersonTemplates(schemeService);
            }
            else if (fraction == Fractions.MilitiaFraction)
            {
                return GetMilitiaPersonTemplates(schemeService);
            }
            else if (fraction == Fractions.TroublemakerFraction)
            {
                return GetTroublemakerPersonTemplates(schemeService);
            }
            else
            {
                return GetPlayerPersonTemplates(schemeService);
            }
        }

        private sealed class PersonTemplate : IPersonTemplateScheme
        {
            public ILocalizedString Name { get; set; }
            public IDropTableScheme HeadEquipments { get; set; }
            public IDropTableScheme BodyEquipments { get; set; }
            public IDropTableScheme MainHandEquipments { get; set; }
            public IDropTableScheme OffHandEquipments { get; set; }
            public IDropTableScheme InventoryProps { get; set; }
            public string FractionSid { get; }
            public string Sid { get; set; }
            public bool Disabled { get; }
            public LocalizedStringSubScheme Description { get; }
            LocalizedStringSubScheme IScheme.Name { get; }
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

        private static IPersonTemplateScheme[] GetInterventionalistsPersonTemplates(ISchemeService schemeService)
        {
            return schemeService.GetSchemes<IPersonTemplateScheme>().Where(x => x.FractionSid == "interventionists").ToArray();
        }

        private static IPersonTemplateScheme[] GetTroublemakerPersonTemplates(ISchemeService schemeService)
        {
            return schemeService.GetSchemes<IPersonTemplateScheme>().Where(x => x.FractionSid == "troublemakers").ToArray();
        }

        private static IPersonTemplateScheme[] GetMilitiaPersonTemplates(ISchemeService schemeService)
        {
            return schemeService.GetSchemes<IPersonTemplateScheme>().Where(x => x.FractionSid == "militia").ToArray();
        }

        private static IPersonTemplateScheme[] GetPlayerPersonTemplates(ISchemeService schemeService)
        {
            return schemeService.GetSchemes<IPersonTemplateScheme>().Where(x => x.FractionSid == "player").ToArray();
        }
    }
}