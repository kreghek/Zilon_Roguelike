using System;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using TechTalk.SpecFlow;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Specs.Steps
{
    [Binding]
    public sealed class SurvivalSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public SurvivalSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Given(@"Актёр значение (.*) равное (.*)")]
        public void GivenАктёрЗначениеСытостьРавное(string statName, int statValue)
        {
            var actor = Context.GetActiveActor();
            var survival = actor.Person.GetModule<ISurvivalModule>();

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
            var actor = Context.GetActiveActor();

            GetEffectStatAndLevelByName(startEffect,
                out SurvivalStatType statType,
                out SurvivalStatHazardLevel effectLevel);

            // Use single to control effect. If effect is incorrect there will be exception.
            var targetStat = actor.Person.GetModuleSafe<ISurvivalModule>()?.Stats?.Single(x => x.Type == statType);
            var keySegment = targetStat.KeySegments.Single(x => x.Level == effectLevel);

            var statValue = keySegment.End;
            targetStat.SetShare(statValue);
        }

        [Given(@"В инвентаре у актёра есть еда: (.*) количество: (.*)")]
        public void GivenВИнвентареУАктёраЕстьЕдаСыр(string propSid, int count)
        {
            var actor = Context.GetActiveActor();
            Context.AddResourceToActor(propSid, count, actor);
        }

        [Given(@"В инвентаре у актёра есть фейковый провиант (.*) \((сытость|вода|хп)\)")]
        [Given(@"В инвентаре у актёра есть фейковый провиант (.*) \((\-сытость|\-вода|\-хп)\)")]
        public void GivenВИнвентареУАктёраЕстьФейковыйПровиантFake_FoodНаХарактеристикуЭффективностью(
            string propSid,
            string provisionStat)
        {
            var actor = Context.GetActiveActor();
            PersonRuleDirection direction;
            ConsumeCommonRuleType consumeRuleType;
            ParseProvisionStat(provisionStat, out direction, out consumeRuleType);

            TestPropScheme propScheme = CreateTestPropScheme(propSid, direction, consumeRuleType);

            FeatureContextBase.AddResourceToActor(propScheme, 1, actor);
        }

        [Then(@"Актёр под эффектом (.*)")]
        public void ThenАктёрПодЭффектом(string effectName)
        {
            var actor = Context.GetActiveActor();

            GetEffectStatAndLevelByName(effectName,
                out SurvivalStatType stat,
                out SurvivalStatHazardLevel level);

            if (stat != SurvivalStatType.Undefined)
            {
                var effect = actor.Person.GetModule<IEffectsModule>().Items
                    .OfType<SurvivalStatHazardEffect>()
                    .SingleOrDefault(x => x.Type == stat);

                effect.Should().NotBeNull();
                effect.Level.Should().Be(level);
            }
            else
            {
                var effects = actor.Person.GetModule<IEffectsModule>().Items.OfType<SurvivalStatHazardEffect>();
                effects.Should().BeEmpty();
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
        public void ThenЗначениеСытостиПовысилосьНаЕдиниц(
            string stat,
            int satietyValue,
            int hungerRate,
            int expectedValue)
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

        public async Task WaitForIteration(int timeUnitCount)
        {
            var globe = Context.Globe;
            var humatTaskSource = Context.ServiceProvider
                .GetRequiredService<IHumanActorTaskSource<ISectorTaskSourceContext>>();
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();

            var counter = timeUnitCount;

            var survivalModule = playerState.ActiveActor?.Actor.Person?.GetModuleSafe<ISurvivalModule>();

            var isPlayerActor = playerState.ActiveActor?.Actor != null;
            if (isPlayerActor)
            {
                while (counter > 0)
                {
                    for (var i = 0; i < GlobeMetrics.OneIterationLength; i++)
                    {
                        if (humatTaskSource.CanIntent() && (survivalModule?.IsDead == false))
                        {
                            var idleCommand = Context.ServiceProvider.GetRequiredService<NextTurnCommand>();
                            idleCommand.Execute();
                        }

                        await globe.UpdateAsync().TimeoutAfter(1000).ConfigureAwait(false);
                    }

                    counter--;
                }
            }
            else
            {
                while (counter > 0)
                {
                    for (var i = 0; i < GlobeMetrics.OneIterationLength; i++)
                    {
                        await globe.UpdateAsync().TimeoutAfter(1000).ConfigureAwait(false);
                    }

                    counter--;
                }
            }
        }

        [When(@"Актёр использует предмет (.*) на себя (\d+) раз")]
        public async Task WhenАctorUsePropNTimesAsync(string propSid, int times)
        {
            for (var i = 0; i < times; i++)
            {
                Context.UsePropByActiveActor(propSid);
                await WaitForIteration(1).ConfigureAwait(false);
            }
        }

        [When(@"Актёр использует предмет (.*) на себя")]
        public void WhenАктёрСъедаетЕду(string propSid)
        {
            Context.UsePropByActiveActor(propSid);
        }

        [When(@"Я перемещаю персонажа на (.*) клетку")]
        public void WhenЯПеремещаюПерсонажаНаОднуКлетку(int moveCount)
        {
            var targetCoords = new[]
            {
                new OffsetCoords(1, 0),
                new OffsetCoords(0, 0)
            };

            for (var i = 0; i < moveCount; i++)
            {
                Context.MoveOnceActiveActor(targetCoords[i % 2]);
            }
        }

        private static TestPropScheme CreateTestPropScheme(
            string propSid,
            PersonRuleDirection direction,
            ConsumeCommonRuleType consumeRuleType)
        {
            return new TestPropScheme
            {
                Sid = propSid,
                Use = new TestPropUseSubScheme
                {
                    Consumable = true,
                    CommonRules = new[]
                    {
                        new ConsumeCommonRule(consumeRuleType, PersonRuleLevel.Lesser, direction)
                    }
                }
            };
        }

        private static void GetEffectStatAndLevelByName(
            string effectName,
            out SurvivalStatType stat,
            out SurvivalStatHazardLevel level)
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

                case "Слабая токсикация":
                    level = SurvivalStatHazardLevel.Lesser;
                    stat = SurvivalStatType.Intoxication;
                    break;

                case "Сильная токсикация":
                    level = SurvivalStatHazardLevel.Strong;
                    stat = SurvivalStatType.Intoxication;
                    break;

                case "Смертельная токсикация":
                    level = SurvivalStatHazardLevel.Max;
                    stat = SurvivalStatType.Intoxication;
                    break;

                case "Слабая рана":
                    level = SurvivalStatHazardLevel.Lesser;
                    stat = SurvivalStatType.Health;
                    break;

                case "Сильная рана":
                    level = SurvivalStatHazardLevel.Strong;
                    stat = SurvivalStatType.Health;
                    break;

                case "Смертельная рана":
                    level = SurvivalStatHazardLevel.Max;
                    stat = SurvivalStatType.Health;
                    break;

                default:
                    throw new NotSupportedException("Неизветный тип ожидаемого эффекта.");
            }
        }

        private static int? GetSurvivalValue(IActor actor, SurvivalStatType type)
        {
            var stat = actor.Person.GetModule<ISurvivalModule>().Stats.SingleOrDefault(x => x.Type == type);
            return stat?.Value;
        }

        private static void ParseProvisionStat(
            string provisionStat,
            out PersonRuleDirection direction,
            out ConsumeCommonRuleType consumeRuleType)
        {
            direction = PersonRuleDirection.Positive;
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
        }
    }
}