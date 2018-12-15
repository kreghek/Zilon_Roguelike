using System.Linq;

using FluentAssertions;

using JetBrains.Annotations;

using LightInject;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Spec.Contexts;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Spec.Steps
{
    [UsedImplicitly]
    [Binding]
    public sealed class EquipmentSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public EquipmentSteps(CommonGameActionsContext context) : base(context)
        {

        }

        [UsedImplicitly]
        [Given(@"В инвентаре у актёра игрока есть предмет: (.*)")]
        public void GivenВИнвентареУАктёраИгрокаЕстьПредметPropSid(string propSid)
        {
            var actor = Context.GetActiveActor();

            var equipment = Context.CreateEquipment(propSid);

            actor.Person.Inventory.Add(equipment);
        }

        [UsedImplicitly]
        [Given(@"Актёр игрока экипирован предметом (.*) в слот Index: (.*)")]
        public void GivenАктёрИгрокаЭкипированНет(string propSid, int slotIndex)
        {
            if (propSid == "нет")
            {
                return;
            }

            var equipment = Context.CreateEquipment(propSid);

            var actor = Context.GetActiveActor();
            actor.Person.EquipmentCarrier.SetEquipment(equipment, slotIndex);
        }


        [UsedImplicitly]
        [When(@"Экипирую предмет (.*) в слот Index: (.*)")]
        public void WhenЭкипируюПредметPropSidВСлотIndexSlotIndex(string propSid, int slotIndex)
        {
            var equipCommand = Context.Container.GetInstance<ICommand>("equip");
            var inventoryState = Context.Container.GetInstance<IInventoryState>();

            ((EquipCommand)equipCommand).SlotIndex = slotIndex;

            var actor = Context.GetActiveActor();

            var targetEquipment = actor.Person.Inventory.CalcActualItems().First(x=>x.Scheme.Sid == propSid);

            var targetEquipmentVeiwModel = new TestPropItemViewModel
            {
                Prop = targetEquipment
            };

            inventoryState.SelectedProp = targetEquipmentVeiwModel;

            equipCommand.Execute();
        }

        [UsedImplicitly]
        [Then(@"В слоте Index: (.+) актёра игрока есть (.+)")]
        public void ThenВСлотеIndexSlotIndexАктёраИгрокаЕстьPropSid(int slotIndex, string propSid)
        {
            var actor = Context.GetActiveActor();

            actor.Person.EquipmentCarrier.Equipments[slotIndex].Scheme.Sid.Should().Be(propSid);
        }

        [Then(@"Невозможна экипировка предмета (.+) в слот Index: (.+)")]
        public void ThenНевозможнаЭкипировкаПредметаPistolВСлотIndex(string propSid, int slotIndex)
        {
            var equipCommand = Context.Container.GetInstance<ICommand>("equip");
            var inventoryState = Context.Container.GetInstance<IInventoryState>();

            ((EquipCommand)equipCommand).SlotIndex = slotIndex;

            var actor = Context.GetActiveActor();

            var targetEquipment = actor.Person.Inventory.CalcActualItems().First(x => x.Scheme.Sid == propSid);

            var targetEquipmentVeiwModel = new TestPropItemViewModel
            {
                Prop = targetEquipment
            };

            inventoryState.SelectedProp = targetEquipmentVeiwModel;

            equipCommand.CanExecute().Should().BeFalse();
        }

    }
}