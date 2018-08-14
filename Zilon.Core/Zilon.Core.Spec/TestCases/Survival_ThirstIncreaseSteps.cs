using System;
using TechTalk.SpecFlow;

namespace Zilon.Core.Spec.TestCases
{
    [Binding]
    public class Survival_ThirstIncreaseSteps
    {
        [Then(@"Значение воды уменьшилось на (.*) единицу")]
        public void ThenЗначениеВодыУменьшилосьНаЕдиницу(int p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
