using System.Linq;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.Tactics;
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
        [Given(@"В инвентаре у актёра игрока есть предмет: (.*)")]
        public void GivenВИнвентареУАктёраИгрокаЕстьПредметPropSid(string propSid)
        {
            IActor actor = Context.GetActiveActor();

            Equipment equipment = Context.CreateEquipment(propSid);

            actor.Person.GetModule<IInventoryModule>().Add(equipment);
        }

        [UsedImplicitly]
        [Given(@"Актёр игрока экипирован предметом (.*) в слот Index: (.*)")]
        public void GivenАктёрИгрокаЭкипированНет(string propSid, int slotIndex)
        {
            if (propSid == "нет")
            {
                return;
            }

            Equipment equipment = Context.CreateEquipment(propSid);

            IActor actor = Context.GetActiveActor();
            actor.Person.GetModule<IEquipmentModule>()[slotIndex] = equipment;
        }

        [UsedImplicitly]
        [When(@"Экипирую предмет (.+) в слот Index: (\d+)")]
        public void WhenЭкипируюПредметPropSidВСлотIndexSlotIndex(string propSid, int slotIndex)
        {
            var equipCommand = Context.ServiceProvider.GetRequiredService<EquipCommand>();
            var inventoryState = Context.ServiceProvider.GetRequiredService<IInventoryState>();

            equipCommand.SlotIndex = slotIndex;

            IActor actor = Context.GetActiveActor();

            IProp targetEquipment = actor.Person.GetModule<IInventoryModule>().CalcActualItems()
                .First(x => x.Scheme.Sid == propSid);

            TestPropItemViewModel targetEquipmentVeiwModel = new TestPropItemViewModel {Prop = targetEquipment};

            inventoryState.SelectedProp = targetEquipmentVeiwModel;

            equipCommand.Execute();
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

        [Then(@"В слоте Index: (\d+) актёра игрока есть (.+)")]
        public void ThenВСлотеIndexSlotIndexАктёраИгрокаЕстьPropSid(int slotIndex, string propSid)
        {
            IActor actor = Context.GetActiveActor();

            actor.Person.GetModule<IEquipmentModule>()[slotIndex].Scheme.Sid.Should().Be(propSid);
        }

        [Then(@"В слоте Index: (\d+) актёра игрока ничего нет")]
        public void ThenВСлотеIndexSlotIndexАктёраИгрокаЕстьPropSid(int slotIndex)
        {
            IActor actor = Context.GetActiveActor();

            actor.Person.GetModule<IEquipmentModule>()[slotIndex].Should().BeNull();
        }

        [Then(@"Невозможна экипировка предмета (.+) в слот Index: (.+)")]
        public void ThenНевозможнаЭкипировкаПредметаPistolВСлотIndex(string propSid, int slotIndex)
        {
            var equipCommand = Context.ServiceProvider.GetRequiredService<EquipCommand>();
            var inventoryState = Context.ServiceProvider.GetRequiredService<IInventoryState>();

            equipCommand.SlotIndex = slotIndex;

            IActor actor = Context.GetActiveActor();

            IProp targetEquipment = actor.Person.GetModule<IInventoryModule>().CalcActualItems()
                .First(x => x.Scheme.Sid == propSid);

            TestPropItemViewModel targetEquipmentVeiwModel = new TestPropItemViewModel {Prop = targetEquipment};

            inventoryState.SelectedProp = targetEquipmentVeiwModel;

            equipCommand.CanExecute().Should().BeFalse();
        }

        [Then(@"Текущий запас здоровья персонажа игрока равен (\d+)")]
        public void ThenЗдоровьеПерсонажаИгрокаРавно(int expectedHp)
        {
            IActor actor = Context.GetActiveActor();

            actor.Person.GetModule<ISurvivalModule>().Stats.Single(x => x.Type == SurvivalStatType.Health).Value
                .Should().Be(expectedHp);
        }

        [Then(@"Максимальный запас здоровья персонажа игрока равен (\d+)")]
        public void ThenМаксимальноеЗдоровьеПерсонажаИгрокаРавно(int expectedMaxHp)
        {
            IActor actor = Context.GetActiveActor();

            actor.Person.GetModule<ISurvivalModule>().Stats.Single(x => x.Type == SurvivalStatType.Health)
                .Range.Max
                .Should().Be(expectedMaxHp);
        }
    }
}