using System;
using System.Linq;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using TechTalk.SpecFlow;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Specs.Steps
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
            var actor = Context.GetActiveActor();
            Context.AddResourceToActor(propSid, count, actor);
        }

        [Given(@"В инвентаре у актёра есть фейковый провиант (.*) \((сытость|вода|хп)\)")]
        [Given(@"В инвентаре у актёра есть фейковый провиант (.*) \((\-сытость|\-вода|\-хп)\)")]
        public void GivenВИнвентареУАктёраЕстьФейковыйПровиантFake_FoodНаХарактеристикуЭффективностью(string propSid,
            string provisionStat)
        {
            var actor = Context.GetActiveActor();

            ConsumeCommonRuleType consumeRuleType;
            var direction = PersonRuleDirection.Positive;

            switch (provisionStat)
            {
                case "сытость":
                    consumeRuleType = ConsumeCommonRuleType.Satiety;
                    break;

                case "вода":
                    consumeRuleType = ConsumeCommonRuleType.Thirst;
                    break;

                case "хп":
                    consumeRuleType = ConsumeCommonRuleType.Health;
                    break;

                case "-сытость":
                    consumeRuleType = ConsumeCommonRuleType.Satiety;
                    direction = PersonRuleDirection.Negative;
                    break;

                case "-вода":
                    consumeRuleType = ConsumeCommonRuleType.Thirst;
                    direction = PersonRuleDirection.Negative;
                    break;

                case "-хп":
                    consumeRuleType = ConsumeCommonRuleType.Health;
                    direction = PersonRuleDirection.Negative;
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }

            var propScheme = new TestPropScheme
            {
                Sid = propSid,
                Use = new TestPropUseSubScheme
                {
                    Consumable = true,
                    CommonRules = new[] {
                        new ConsumeCommonRule(consumeRuleType, PersonRuleLevel.Lesser, direction)
                    }
                }
            };

            Context.AddResourceToActor(propScheme, 1, actor);
        }

        [Given(@"Актёр значение (.*) равное (.*)")]
        public void GivenАктёрЗначениеСытостьРавное(string statName, int statValue)
        {
            var actor = Context.GetActiveActor();
            var survival = actor.Person.Survival;

            SurvivalStatType statType;
            switch (statName)
            {
                case "сытость":
                    statType = SurvivalStatType.Satiety;
                    break;

                case "вода":
                    statType = SurvivalStatType.Hydration;
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }

            survival.SetStatForce(statType, statValue);
        }

        [Given(@"Актёр имеет эффект (.*)")]
        public void GivenАктёрИмеетЭффектStartEffect(string startEffect)
        {
            var survivalRandomSource = Context.ServiceProvider.GetRequiredService<ISurvivalRandomSource>();

            var actor = Context.GetActiveActor();

            GetEffectStatAndLevelByName(startEffect,
                out SurvivalStatType stat,
                out SurvivalStatHazardLevel level);

            var effect = new SurvivalStatHazardEffect(stat, level, survivalRandomSource);

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
                Context.MoveOnceActiveActor(targetCoords[i % 2]);
            }
        }

        [When(@"Актёр использует предмет (.*) на себя")]
        public void WhenАктёрСъедаетЕду(string propSid)
        {
            Context.UsePropByActiveActor(propSid);
        }

        [Then(@"Значение (сытость|вода) уменьшилось на (.*) и стало (.*)")]
        public void ThenЗначениеStatУменьшилосьНаRate(string stat, int hungerRate, int expectedValue)
        {
            var actor = Context.GetActiveActor();

            switch (stat)
            {
                case "сытость":
                    GetSurvivalValue(actor, SurvivalStatType.Satiety).Should().Be(expectedValue);
                    break;

                case "вода":
                    GetSurvivalValue(actor, SurvivalStatType.Hydration).Should().Be(expectedValue);
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }
        }

        [Then(@"Значение (сытость|вода) повысилось на (.*) и уменьшилось на (.*) за игровой цикл и стало (.*)")]
        public void ThenЗначениеСытостиПовысилосьНаЕдиниц(string stat, int satietyValue, int hungerRate, int expectedValue)
        {
            var actor = Context.GetActiveActor();

            switch (stat)
            {
                case "сытость":
                    GetSurvivalValue(actor, SurvivalStatType.Satiety).Should().Be(expectedValue);
                    break;

                case "вода":
                    GetSurvivalValue(actor, SurvivalStatType.Hydration).Should().Be(expectedValue);
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }
        }

        [Then(@"Значение (сытость|вода) стало (.*)")]
        public void ThenЗначениеStatСтало(string stat, int expectedValue)
        {
            var actor = Context.GetActiveActor();

            int? survivalStatValue;
            switch (stat)
            {
                case "сытость":
                    survivalStatValue = GetSurvivalValue(actor, SurvivalStatType.Satiety);
                    break;

                case "вода":
                    survivalStatValue = GetSurvivalValue(actor, SurvivalStatType.Hydration);
                    break;

                default:
                    throw new NotSupportedException("Передан неподдерживаемый тип характеристики.");
            }

            survivalStatValue.Should().Be(expectedValue);
        }

        [Then(@"Актёр под эффектом (.*)")]
        public void ThenАктёрПолучаетЭффектСлабыйГолод(string effectName)
        {
            var actor = Context.GetActiveActor();

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
                    stat = SurvivalStatType.Hydration;
                    break;

                case "Жажда":
                    level = SurvivalStatHazardLevel.Strong;
                    stat = SurvivalStatType.Hydration;
                    break;

                case "Обезвоживание":
                    level = SurvivalStatHazardLevel.Max;
                    stat = SurvivalStatType.Hydration;
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
