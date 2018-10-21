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
            var actor = _context.GetActiveActor();

            var equipment = _context.CreateEquipment(propSid);

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

            var equipment = _context.CreateEquipment(propSid);

            var actor = _context.GetActiveActor();
            actor.Person.EquipmentCarrier.SetEquipment(equipment, slotIndex);
        }


        [UsedImplicitly]
        [When(@"Экипирую предмет (.*) в слот Index: (.*)")]
        public void WhenЭкипируюПредметPropSidВСлотIndexSlotIndex(string propSid, int slotIndex)
        {
            var equipCommand = _context.Container.GetInstance<ICommand>("equip");
            var inventoryState = _context.Container.GetInstance<IInventoryState>();

            ((EquipCommand)equipCommand).SlotIndex = slotIndex;

            var actor = _context.GetActiveActor();

            var targetEquipment = actor.Person.Inventory.CalcActualItems().Single(x=>x.Scheme.Sid == propSid);

            var targetEquipmentVeiwModel = new TestPropItemViewModel
            {
                Prop = targetEquipment
            };

            inventoryState.SelectedProp = targetEquipmentVeiwModel;

            equipCommand.Execute();
        }

        [UsedImplicitly]
        [Then(@"В слоте Index: (.*) актёра игрока есть (.*)")]
        public void ThenВСлотеIndexSlotIndexАктёраИгрокаЕстьPropSid(int slotIndex, string propSid)
        {
            var actor = _context.GetActiveActor();

            actor.Person.EquipmentCarrier.Equipments[slotIndex].Scheme.Sid.Should().Be(propSid);
        }
    }
}