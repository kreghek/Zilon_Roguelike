using System;
using TechTalk.SpecFlow;

namespace Zilon.Core.Spec.TestCases
{
    [Binding]
    public class Survival_HungerIncreaseSteps
    {
        [Given(@"Есть произвольная карта")]
        public void GivenЕстьПроизвольнаяКарта()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"Есть персонаж игрока")]
        public void GivenЕстьПерсонажИгрока()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"Я перемещаю персонажа на одну клетку")]
        public void WhenЯПеремещаюПерсонажаНаОднуКлетку()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"Значение сытости уменьшилось на (.*) единицу")]
        public void ThenЗначениеСытостиУменьшилосьНаЕдиницу(int p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
