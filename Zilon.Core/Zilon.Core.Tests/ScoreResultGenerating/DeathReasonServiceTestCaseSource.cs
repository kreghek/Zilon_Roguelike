using System;
using System.Collections;
using System.Linq;

using Moq;

using NUnit.Framework;

using Zilon.Core.Localization;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.ScoreResultGenerating.Tests
{
    public static class DeathReasonServiceTestCaseSource
    {
        public static IEnumerable TestCases
        {
            get
            {
                var languages = Enum.GetValues(typeof(Language)).Cast<Language>().Where(x => x != Language.Undefined)
                    .ToArray();
                var effectTypes = new[]
                    { SurvivalStatType.Hydration, SurvivalStatType.Intoxication, SurvivalStatType.Satiety };

                var schemeService = CreateSchemeService();
                var monsterSchemes = schemeService.GetSchemes<IMonsterScheme>();

                foreach (var language in languages)
                {
                    foreach (var effectType in effectTypes)
                    {
                        var effect = new SurvivalStatHazardEffect(
                            effectType,
                            SurvivalStatHazardLevel.Max,
                            Mock.Of<ISurvivalRandomSource>());
                        var playerEvent = new SurvivalEffectDamageEvent(effect);
                        yield return new TestCaseData(playerEvent, language);
                    }

                    foreach (var monsterScheme in monsterSchemes)
                    {
                        if (monsterScheme.Sid == "default")
                        {
                            continue;
                        }

                        var act = Mock.Of<ITacticalAct>();
                        var monster = new MonsterPerson(monsterScheme);
                        var monsterActor = Mock.Of<IActor>(x => x.Person == monster);
                        var playerEvent = new PlayerDamagedEvent(act, monsterActor);

                        yield return new TestCaseData(playerEvent, language);
                    }
                }
            }
        }

        private static ISchemeService CreateSchemeService()
        {
            var schemeLocator = FileSchemeLocator.CreateFromEnvVariable();

            var schemeHandlerFactory = new StrictSchemeServiceHandlerFactory(schemeLocator);

            return new SchemeService(schemeHandlerFactory);
        }
    }
}