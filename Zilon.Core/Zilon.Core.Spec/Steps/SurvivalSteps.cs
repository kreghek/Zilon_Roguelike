using System;
using System.Linq;

using FluentAssertions;

using TechTalk.SpecFlow;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Spec.Contexts;
using Zilon.Core.Tactics;

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
        [Obsolete]
        public void GivenЕстьПроизвольнаяКарта()
        {
            //TODO Убрать этот метод, актуализировать шаги.
            _context.CreateSector(2);
        }

        [Given(@"Есть карта размером (.*)")]
        public void GivenЕстьКартаРазмером(int mapSize)
        {
            _context.CreateSector(mapSize);
        }


        [Given(@"Есть актёр игрока")]
        [Obsolete]
        public void GivenЕстьПерсонажИгрока()
        {
            //TODO Убрать этот метод, актуализировать шаги.
            _context.AddHumanActor("captain", new OffsetCoords(0, 0));
        }

        [Given(@"Есть актёр игрока класса (.*) в ячейке \((.*), (.*)\)")]
        public void GivenЕстьАктёрИгрокаКлассаCaptainВЯчейке(string personSid, int nodeX, int nodeY)
        {
            _context.AddHumanActor(personSid, new OffsetCoords(nodeX, nodeY));
        }


        [Given(@"В инвентаре у актёра есть еда: (.*) количество: (.*)")]
        public void GivenВИнвентареУАктёраЕстьЕдаСыр(string propSid, int count)
        {
            var actor = _context.GetActiveActor();
            _context.AddResourceToActor(propSid, count, actor);
        }

        [Given(@"Актёр значение (.*) равное (.*)")]
        public void GivenАктёрЗначениеСытостьРавное(string statName, int statValue)
        {
            var actor = _context.GetActiveActor();

            SurvivalStat stat;

            switch (statName)
            {
                case "сытость":
                    stat = actor.Person.Survival.Stats.SingleOrDefault(x=>x.Type == SurvivalStatType.Satiety);
                    break;

                case "вода":
                    stat = actor.Person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Water);
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }

            stat.Value = statValue;
        }

        [Given(@"Актёр имеет эффект (.*)")]
        public void GivenАктёрИмеетЭффектStartEffect(string startEffect)
        {
            var actor = _context.GetActiveActor();

            GetEffectStatAndLevelByName(startEffect,
                out SurvivalStatType stat,
                out SurvivalStatHazardLevel level);

            var effect = new SurvivalStatHazardEffect(stat, level);

            actor.Person.Effects.Add(effect);
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

        [Then(@"Значение (сытость|вода) уменьшилось на (.*) и стало (.*)")]
        public void ThenЗначениеStatУменьшилосьНаRate(string stat, int hungerRate, int expectedValue)
        {
            var actor = _context.GetActiveActor();

            switch (stat)
            {
                case "сытость":
                    GetSurvivalValue(actor, SurvivalStatType.Satiety).Should().Be(expectedValue);
                    break;

                case "вода":
                    GetSurvivalValue(actor, SurvivalStatType.Water).Should().Be(expectedValue);
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }
        }

        [Then(@"Значение (сытость|вода) повысилось на (.*) и уменьшилось на (.*) за игровой цикл и стало (.*)")]
        public void ThenЗначениеСытостиПовысилосьНаЕдиниц(string stat, int satietyValue, int hungerRate, int expectedValue)
        {
            var actor = _context.GetActiveActor();

            switch (stat)
            {
                case "сытость":
                    GetSurvivalValue(actor, SurvivalStatType.Satiety).Should().Be(expectedValue);
                    break;

                case "вода":
                    GetSurvivalValue(actor, SurvivalStatType.Water).Should().Be(expectedValue);
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }
        }

        [Then(@"Значение (сытость|вода) стало (.*)")]
        public void ThenЗначениеStatСтало(string stat, int expectedValue)
        {
            var actor = _context.GetActiveActor();
            switch (stat)
            {
                case "сытость":
                    GetSurvivalValue(actor, SurvivalStatType.Satiety).Should().Be(expectedValue);
                    break;

                case "вода":
                    GetSurvivalValue(actor, SurvivalStatType.Water).Should().Be(expectedValue);
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

        [Then(@"Актёр под эффектом (.*)")]
        public void ThenАктёрПолучаетЭффектСлабыйГолод(string effectName)
        {
            var actor = _context.GetActiveActor();

            GetEffectStatAndLevelByName(effectName, 
                out SurvivalStatType stat, 
                out SurvivalStatHazardLevel level);

            if (stat != SurvivalStatType.Undefined)
            {
                var effect = actor.Person.Effects.Items.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == stat);
                effect.Should().NotBeNull();
                effect.Level.Should().Be(level);
            }
            else
            {
                var effect = actor.Person.Effects.Items.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault();
                effect.Should().BeNull();
            }
        }

        [Then(@"Актёр имеет задас hp (.*)")]
        public void ThenАктёрИмеетЗадасHp(int expectedHp)
        {
            var actor = _context.GetActiveActor();
            actor.State.Hp.Should().Be(expectedHp);
        }

        [Then(@"Актёр имеет характристику модуля сражения (.*) равную (.*)")]
        public void ThenАктёрИмеетХарактристикуМодуляСраженияMeleeРавную(string combatStatName, int combatStatValue)
        {
            var actor = _context.GetActiveActor();

            CombatStatType statType;
            switch (combatStatName)
            {
                case "melee":
                    statType = CombatStatType.Melee;
                    break;

                default:
                    throw new NotSupportedException("Неизвестный тип характеристики модуля сражения.");
            }

            var combatStat = actor.Person.CombatStats.Stats.SingleOrDefault(x=>x.Stat == statType);
            combatStat.Value.Should().Be(combatStatValue);
        }



        private static void GetEffectStatAndLevelByName(string effectName, out SurvivalStatType stat, out SurvivalStatHazardLevel level)
        {
            switch (effectName)
            {
                case "нет":
                    level = SurvivalStatHazardLevel.Undefined;
                    stat = SurvivalStatType.Undefined;
                    break;

                case "Слабый голод":
                    level = SurvivalStatHazardLevel.Lesser;
                    stat = SurvivalStatType.Satiety;
                    break;

                case "Голод":
                    level = SurvivalStatHazardLevel.Strong;
                    stat = SurvivalStatType.Satiety;
                    break;

                case "Голодание":
                    level = SurvivalStatHazardLevel.Max;
                    stat = SurvivalStatType.Satiety;
                    break;

                case "Слабая жажда":
                    level = SurvivalStatHazardLevel.Lesser;
                    stat = SurvivalStatType.Water;
                    break;

                case "Жажда":
                    level = SurvivalStatHazardLevel.Strong;
                    stat = SurvivalStatType.Water;
                    break;

                case "Обезвоживание":
                    level = SurvivalStatHazardLevel.Max;
                    stat = SurvivalStatType.Water;
                    break;

                default:
                    throw new NotSupportedException("Неизветный тип ожидаемого эффекта.");
            }
        }

        private int? GetSurvivalValue(IActor actor, SurvivalStatType type)
        {
            var stat = actor.Person.Survival.Stats.SingleOrDefault(x => x.Type == type);
            return stat?.Value;
        }
    }
}
