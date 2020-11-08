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
            /*return new[] {
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
            };*/
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
    }
}