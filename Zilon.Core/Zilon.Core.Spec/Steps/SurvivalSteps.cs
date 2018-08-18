using System;
using System.Linq;

using FluentAssertions;

using TechTalk.SpecFlow;

using Zilon.Core.Spec.Contexts;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public class SurvivalSteps
    {
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

        [Given(@"Есть актёр игрока")]
        public void GivenЕстьПерсонажИгрока()
        {
            _context.AddHumanActor(new OffsetCoords(0, 0));
        }

        [Given(@"В инвентаре у актёра есть еда: (.*) количество: (.*)")]
        public void GivenВИнвентареУАктёраЕстьЕдаСыр(string propSid, int count)
        {
            var actor = _context.GetActiveActor();
            _context.AddResourceToActor(propSid, count, actor);
        }

        [When(@"Я перемещаю персонажа на (.*) клетку")]
        public void WhenЯПеремещаюПерсонажаНаОднуКлетку(int moveCount)
        {
            var targetCoords = new[] {
                new OffsetCoords(1, 0),
                new OffsetCoords(0, 0)
            };

            for (var i = 0; i < moveCount; i++)
            {
                _context.MoveOnceActiveActor(targetCoords[i % 2]);
            }
        }

        [When(@"Актёр использует предмет (.*) на себя")]
        public void WhenАктёрСъедаетЕду(string propSid)
        {
            _context.UsePropByActiveActor(propSid);
        }

        [Then(@"Значение (сытость|вода) уменьшилось на (.*) единицу и стало (.*)")]
        public void ThenЗначениеStatУменьшилосьНаRate(string stat, int hungerRate, int expectedValue)
        {
            var actor = _context.GetActiveActor();

            switch (stat)
            {
                case "сытость":
                    actor.Person.Survival.Satiety.Should().Be(expectedValue);
                    break;
                case "вода":
                    actor.Person.Survival.Thirst.Should().Be(expectedValue);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        [Then(@"Значение (сытость|вода) повысилось на (.*) единиц и уменьшилось на (.*) за игровой цикл и стало (.*)")]
        public void ThenЗначениеСытостиПовысилосьНаЕдиниц(string stat, int satietyValue, int hungerRate, int expectedSatiety)
        {
            var actor = _context.GetActiveActor();

            switch (stat)
            {
                case "сытость":
                    actor.Person.Survival.Satiety.Should().Be(expectedSatiety);
                    break;

                case "вода":
                    actor.Person.Survival.Thirst.Should().Be(expectedSatiety);
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }
        }

        [Then(@"Значение (сытость|вода) стало (.*)")]
        public void ThenЗначениеStatСтало(string stat, int expectedSatiety)
        {
            var actor = _context.GetActiveActor();
            switch (stat)
            {
                case "сытость":
                    actor.Person.Survival.Satiety.Should().Be(expectedSatiety);
                    break;

                case "вода":
                    actor.Person.Survival.Thirst.Should().Be(expectedSatiety);
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }
        }


        [Then(@"Предмет (.*) отсутствует в инвентаре актёра")]
        public void ThenЕдаСырОтсутствуетВИнвентареПерсонажа(string propSid)
        {
            var actor = _context.GetActiveActor();
            var propsInInventory = actor.Person.Inventory.CalcActualItems();
            var testedProp = propsInInventory.FirstOrDefault(x => x.Scheme.Sid == propSid);
            testedProp.Should().BeNull();
        }

        [Then(@"Актёр получает эффект (.*)")]
        public void ThenАктёрПолучаетЭффектСлабыйГолод(string effectName)
        {
            ScenarioContext.Current.Pending();
        }

    }
}
