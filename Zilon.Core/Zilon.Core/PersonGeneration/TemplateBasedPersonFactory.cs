using System.Linq;

using Zilon.Core.CommonServices.Dices;
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
            IDice dice) : base(schemeService, survivalRandomSource, propFactory, dropResolver, personPerkInitializator,
            dice)
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

        private static IPersonTemplateScheme[] GetInterventionalistsPersonTemplates(ISchemeService schemeService)
        {
            return schemeService.GetSchemes<IPersonTemplateScheme>().Where(x => x.FractionSid == "interventionists")
                .ToArray();
        }

        private static IPersonTemplateScheme[] GetMilitiaPersonTemplates(ISchemeService schemeService)
        {
            return schemeService.GetSchemes<IPersonTemplateScheme>().Where(x => x.FractionSid == "militia").ToArray();
        }

        private static IPersonTemplateScheme[] GetPersonTemplateByFraction(IFraction fraction,
            ISchemeService schemeService)
        {
            if (fraction == Fractions.InterventionistFraction)
            {
                return GetInterventionalistsPersonTemplates(schemeService);
            }

            if (fraction == Fractions.MilitiaFraction)
            {
                return GetMilitiaPersonTemplates(schemeService);
            }

            if (fraction == Fractions.TroublemakerFraction)
            {
                return GetTroublemakerPersonTemplates(schemeService);
            }

            return GetPlayerPersonTemplates(schemeService);
        }

        private static IPersonTemplateScheme[] GetPlayerPersonTemplates(ISchemeService schemeService)
        {
            return schemeService.GetSchemes<IPersonTemplateScheme>().Where(x => x.FractionSid == "player").ToArray();
        }

        private static IPersonTemplateScheme[] GetTroublemakerPersonTemplates(ISchemeService schemeService)
        {
            return schemeService.GetSchemes<IPersonTemplateScheme>().Where(x => x.FractionSid == "troublemakers")
                .ToArray();
        }
    }
}