using System.Linq;

using FluentAssertions;

using TechTalk.SpecFlow;
using Zilon.Core.Commands;
using Zilon.Core.Spec.Contexts;

using LightInject;
using Zilon.Core.Client;
using Zilon.Core.Tests.Common;
using Zilon.Core.Persons;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public sealed class EquipmentSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public EquipmentSteps(CommonGameActionsContext context) : base(context)
        {

        }

        [Given(@"В инвентаре у актёра игрока есть предмет: (.*)")]
        public void GivenВИнвентареУАктёраИгрокаЕстьПредметPropSid(string propSid)
        {
            var actor = _context.GetActiveActor();

            var equipment = _context.CreateEquipment(propSid);

            actor.Person.Inventory.Add(equipment);
        }

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

        [Then(@"В слоте Index: (.*) актёра игрока есть (.*)")]
        public void ThenВСлотеIndexSlotIndexАктёраИгрокаЕстьPropSid(int slotIndex, string propSid)
        {
            var actor = _context.GetActiveActor();

            actor.Person.EquipmentCarrier.Equipments[slotIndex].Scheme.Sid.Should().Be(propSid);
        }

        [Then(@"Параметр (.*) равен (.*)")]
        public void ThenПараметрParamStatTypeРавенParamStatValue(string paramType, int paramValue)
        {
            if (paramType == "-")
            {
                return;
            }
            else
            {
                ScenarioContext.Current.Pending();
            }
        }


    }
}