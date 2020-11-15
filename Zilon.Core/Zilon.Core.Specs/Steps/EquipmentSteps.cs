using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Specs.Steps
{
    [UsedImplicitly]
    [Binding]
    public sealed class EquipmentSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public EquipmentSteps(CommonGameActionsContext context) : base(context)
        {
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
            actor.Person.GetModule<IEquipmentModule>()[slotIndex] = equipment;
        }

        [UsedImplicitly]
        [Given(@"В инвентаре у актёра игрока есть предмет: (.*)")]
        public void GivenВИнвентареУАктёраИгрокаЕстьПредметPropSid(string propSid)
        {
            var actor = Context.GetActiveActor();

            var equipment = Context.CreateEquipment(propSid);

            actor.Person.GetModule<IInventoryModule>().Add(equipment);
        }

        [Then(@"В слоте Index: (\d+) актёра игрока есть (.+)")]
        public void ThenВСлотеIndexSlotIndexАктёраИгрокаЕстьPropSid(int slotIndex, string propSid)
        {
            var actor = Context.GetActiveActor();

            actor.Person.GetModule<IEquipmentModule>()[slotIndex].Scheme.Sid.Should().Be(propSid);
        }

        [Then(@"В слоте Index: (\d+) актёра игрока ничего нет")]
        public void ThenВСлотеIndexSlotIndexАктёраИгрокаЕстьPropSid(int slotIndex)
        {
            var actor = Context.GetActiveActor();

            actor.Person.GetModule<IEquipmentModule>()[slotIndex].Should().BeNull();
        }

        [Then(@"Текущий запас здоровья персонажа игрока равен (\d+)")]
        public void ThenЗдоровьеПерсонажаИгрокаРавно(int expectedHp)
        {
            var actor = Context.GetActiveActor();

            actor.Person.GetModule<ISurvivalModule>().Stats.Single(x => x.Type == SurvivalStatType.Health).Value
                .Should().Be(expectedHp);
        }

        [Then(@"Максимальный запас здоровья персонажа игрока равен (\d+)")]
        public void ThenМаксимальноеЗдоровьеПерсонажаИгрокаРавно(int expectedMaxHp)
        {
            var actor = Context.GetActiveActor();

            actor.Person.GetModule<ISurvivalModule>().Stats.Single(x => x.Type == SurvivalStatType.Health)
                .Range.Max
                .Should().Be(expectedMaxHp);
        }

        [Then(@"Невозможна экипировка предмета (.+) в слот Index: (.+)")]
        public void ThenНевозможнаЭкипировкаПредметаPistolВСлотIndex(string propSid, int slotIndex)
        {
            var equipCommand = Context.ServiceProvider.GetRequiredService<EquipCommand>();
            var inventoryState = Context.ServiceProvider.GetRequiredService<IInventoryState>();

            equipCommand.SlotIndex = slotIndex;

            var actor = Context.GetActiveActor();

            var targetEquipment = actor.Person.GetModule<IInventoryModule>().CalcActualItems()
                .First(x => x.Scheme.Sid == propSid);

            var targetEquipmentVeiwModel = new TestPropItemViewModel
            {
                Prop = targetEquipment
            };

            inventoryState.SelectedProp = targetEquipmentVeiwModel;

            equipCommand.CanExecute().Should().BeFalse();
        }

        [When(@"Снимаю экипировку из слота (\d+)")]
        public void WhenСнимаюЭкипировкуИзСлота(int slotIndex)
        {
            var equipCommand = Context.ServiceProvider.GetRequiredService<EquipCommand>();
            var inventoryState = Context.ServiceProvider.GetRequiredService<IInventoryState>();

            equipCommand.SlotIndex = slotIndex;

            inventoryState.SelectedProp = null;

            equipCommand.Execute();
        }

        [UsedImplicitly]
        [When(@"Экипирую предмет (.+) в слот Index: (\d+)")]
        public void WhenЭкипируюПредметPropSidВСлотIndexSlotIndex(string propSid, int slotIndex)
        {
            var equipCommand = Context.ServiceProvider.GetRequiredService<EquipCommand>();
            var inventoryState = Context.ServiceProvider.GetRequiredService<IInventoryState>();

            equipCommand.SlotIndex = slotIndex;

            var actor = Context.GetActiveActor();

            var targetEquipment = actor.Person.GetModule<IInventoryModule>().CalcActualItems()
                .First(x => x.Scheme.Sid == propSid);

            var targetEquipmentVeiwModel = new TestPropItemViewModel
            {
                Prop = targetEquipment
            };

            inventoryState.SelectedProp = targetEquipmentVeiwModel;

            equipCommand.Execute();
        }
    }
}