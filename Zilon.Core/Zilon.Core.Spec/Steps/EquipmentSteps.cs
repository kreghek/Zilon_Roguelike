using System.Linq;

using FluentAssertions;

using TechTalk.SpecFlow;

using Zilon.Core.Spec.Contexts;

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
            ScenarioContext.Current.Pending();
        }

        [When(@"Экипирую предмет (.*) в слот Index: (.*)")]
        public void WhenЭкипируюПредметPropSidВСлотIndexSlotIndex(string propSid, int slotIndex)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"В слоте Index: (.*) актёра игрока есть (.*)")]
        public void ThenВСлотеIndexSlotIndexАктёраИгрокаЕстьPropSid(int slotIndex, string propSid)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"Параметр (.*) равен (.*)")]
        public void ThenПараметрParamStatTypeРавенParamStatValue(string paramType, int paramValue)
        {
            ScenarioContext.Current.Pending();
        }


    }
}