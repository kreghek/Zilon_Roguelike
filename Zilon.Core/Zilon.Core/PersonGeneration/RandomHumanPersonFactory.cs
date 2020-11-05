
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.PersonGeneration
{
    public sealed class RandomHumanPersonFactory : FullPersonFactoryBase
    {
        private const string HEAD_DROP_SID = "start-heads";
        private const string MAIN_WEAPON_DROP_SID = "start-main-weapons";
        private const string BODY_DROP_SID = "start-armors";
        private const string OFF_WEAPON_DROP_SID = "start-off-weapons";
        private const string START_PROP_DROP_SID = "start-props";

        public RandomHumanPersonFactory(
            ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            IDropResolver dropResolver,
            IPersonPerkInitializator personPerkInitializator,
            IDice dice) : base(
                schemeService,
                survivalRandomSource,
                propFactory,
                dropResolver,
                personPerkInitializator,
                dice)
        {
        }

        protected override void RollStartEquipment(IInventoryModule inventory, HumanPerson person)
        {
            var headDropScheme = GetHeads();
            FillSlot(person, headDropScheme, HeadSlotIndex);

            var armorDropScheme = GetArmors();
            FillSlot(person, armorDropScheme, BodySlotIndex);

            var mainWeaponDropScheme = GetMainWeapons();
            FillSlot(person, mainWeaponDropScheme, MainHandSlotIndex);

            var offWeaponDropScheme = GetOffWeapons();
            FillSlot(person, offWeaponDropScheme, OffHandSlotIndex);

            var startPropDropScheme = GetStartProps();
            var startProps = DropResolver.Resolve(new[] { startPropDropScheme });
            foreach (var prop in startProps)
            {
                AddPropToInventory(inventory, prop);
            }

            AddDefaultProps(inventory);
            
            AddEquipment(inventory, "pick-axe");
            AddEquipment(inventory, "shovel");
            AddEquipment(inventory, "shotgun");
            AddResource(inventory, "bullet-45", 100);
        }

        private IDropTableScheme GetHeads()
        {
            return SchemeService.GetScheme<IDropTableScheme>(HEAD_DROP_SID);
        }

        private IDropTableScheme GetMainWeapons()
        {
            return SchemeService.GetScheme<IDropTableScheme>(MAIN_WEAPON_DROP_SID);
        }

        private IDropTableScheme GetArmors()
        {
            return SchemeService.GetScheme<IDropTableScheme>(BODY_DROP_SID);
        }

        private IDropTableScheme GetOffWeapons()
        {
            return SchemeService.GetScheme<IDropTableScheme>(OFF_WEAPON_DROP_SID);
        }

        private IDropTableScheme GetStartProps()
        {
            return SchemeService.GetScheme<IDropTableScheme>(START_PROP_DROP_SID);
        }
    }
}