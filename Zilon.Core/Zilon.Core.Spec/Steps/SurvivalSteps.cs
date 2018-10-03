using System;
using System.Linq;

using FluentAssertions;

using TechTalk.SpecFlow;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Spec.Contexts;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public sealed class SurvivalSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public SurvivalSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Given(@"В инвентаре у актёра есть еда: (.*) количество: (.*)")]
        public void GivenВИнвентареУАктёраЕстьЕдаСыр(string propSid, int count)
        {
            var actor = _context.GetActiveActor();
            _context.AddResourceToActor(propSid, count, actor);
        }

        [Given(@"В инвентаре у актёра есть фейковый провиант (.*) \((сытость|вода|хп) - (.*)\)")]
        public void GivenВИнвентареУАктёраЕстьФейковыйПровиантFake_FoodНаХарактеристикуЭффективностью(string propSid, 
            string provisionStat, 
            int provisitonEfficient)
        {
            var actor = _context.GetActiveActor();

            ConsumeCommonRule consumeRule;

            switch (provisionStat)
            {
                case "сытость":
                    consumeRule = ConsumeCommonRule.Satiety;
                    break;

                case "вода":
                    consumeRule = ConsumeCommonRule.Thrist;
                    break;

                case "хп":
                    consumeRule = ConsumeCommonRule.Health;
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }

            var propScheme = new PropScheme {
                Sid = propSid,
                Use = new TestPropUseSubScheme
                {
                    Consumable = true,
                    CommonRules = new[] { consumeRule },
                    Value = provisitonEfficient
                }
            };

            _context.AddResourceToActor(propScheme, 1, actor);
        }


        [Given(@"Актёр значение (.*) равное (.*)")]
        public void GivenАктёрЗначениеСытостьРавное(string statName, int statValue)
        {
            var actor = _context.GetActiveActor();

            SurvivalStat stat;

            switch (statName)
            {
                case "сытость":
                    stat = actor.Person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Satiety);
                    break;

                case "вода":
                    stat = actor.Person.Survival.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Water);
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }

            if (stat == null)
            {
                throw new InvalidOperationException($"Не найдена характеристика модуля выживания: {statName}.");
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

        [Then(@"Актёр под эффектом (.*)")]
        public void ThenАктёрПолучаетЭффектСлабыйГолод(string effectName)
        {
            var actor = _context.GetActiveActor();

            GetEffectStatAndLevelByName(effectName,
                out SurvivalStatType stat,
                out SurvivalStatHazardLevel level);

            if (stat != SurvivalStatType.Undefined)
            {
                var effect = actor.Person.Effects.Items
                    .OfType<SurvivalStatHazardEffect>()
                    .Single(x => x.Type == stat);

                effect.Should().NotBeNull();
                effect.Level.Should().Be(level);
            }
            else
            {
                var effects = actor.Person.Effects.Items.OfType<SurvivalStatHazardEffect>();
                effects.Should().BeEmpty();
            }
        }

        [Then(@"Актёр имеет характристику модуля сражения (.*) равную (.*)")]
        public void ThenАктёрИмеетХарактристикуМодуляСраженияMeleeРавную(string combatStatName, int combatStatValue)
        {
            ScenarioContext.Current.Pending();
            //var actor = _context.GetActiveActor();

            //SkillStatType statType;
            //switch (combatStatName)
            //{
            //    case "melee":
            //        statType = SkillStatType.Melee;
            //        break;

            //    default:
            //        throw new NotSupportedException($"Неизвестный тип характеристики модуля сражения {combatStatName}.");
            //}

            //var combatStat = actor.Person.CombatStats.Stats.SingleOrDefault(x => x.Stat == statType);

            //if (combatStat == null)
            //{
            //    throw new InvalidOperationException($"Не найдена характеристика модуля сражения {statType}.");
            //}

            //combatStat.Value.Should().Be(combatStatValue);
        }

        [Then(@"Тактическое умение (.*) имеет эффективность Min: (.*) Max: (.*)")]
        public void ThenТактическоеУмениеИмеетЭффективностьMinMax(string tacticalActSid, int minEfficient, int maxEfficient)
        {
            var actor = _context.GetActiveActor();

            var tacticalAct = actor.Person.TacticalActCarrier.Acts.OfType<TacticalAct>()
                .Single(x => x.Scheme.Sid == tacticalActSid);

            tacticalAct.MinEfficient.Should().Be(minEfficient);
            tacticalAct.MaxEfficient.Should().Be(maxEfficient);
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
