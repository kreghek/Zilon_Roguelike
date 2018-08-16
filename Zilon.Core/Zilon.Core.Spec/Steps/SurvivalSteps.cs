using FluentAssertions;
using TechTalk.SpecFlow;

using Zilon.Core.Spec.Contexts;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public class SurvivalSteps
    {
        const int _maxSatiety = 100;
        const int _maxThirst = 100;
        const int _satietyPerTurnRate = 1;
        const int _thirstPerTurnRate = 1;

        private readonly SurvivalContext _context;

        public SurvivalSteps(SurvivalContext context)
        {
            _context = context;
        }

        [Given(@"Есть произвольная карта")]
        public void GivenЕстьПроизвольнаяКарта()
        {
            _context.CreateSector();
        }

        [Given(@"Есть персонаж игрока")]
        public void GivenЕстьПерсонажИгрока()
        {
            _context.AddHumanActor(new OffsetCoords(0, 0));
        }

        [When(@"Я перемещаю персонажа на одну клетку")]
        public void WhenЯПеремещаюПерсонажаНаОднуКлетку()
        {
            _context.MoveOnceActiveActor(new OffsetCoords(1, 0));
        }

        [Then(@"Значение сытости уменьшилось на (.*) единицу")]
        public void ThenЗначениеСытостиУменьшилосьНаЕдиницу(int p0)
        {
            var expectedSatiety = _maxSatiety - _satietyPerTurnRate;

            var actor = _context.GetActiveActor();
            actor.Person.Survival.Satiety.Should().Be(expectedSatiety);
        }

        [Then(@"Значение воды уменьшилось на (.*) единицу")]
        public void ThenЗначениеВодыУменьшилосьНаЕдиницу(int p0)
        {
            var expectedThirst = _maxThirst - _thirstPerTurnRate;

            var actor = _context.GetActiveActor();
            actor.Person.Survival.Thirst.Should().Be(expectedThirst);
        }
    }
}
