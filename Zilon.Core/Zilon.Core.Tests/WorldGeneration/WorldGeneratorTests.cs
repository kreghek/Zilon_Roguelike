using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Schemes;
using Zilon.Core.WorldGeneration;
using Zilon.Core.WorldGeneration.AgentMemories;

namespace Zilon.Core.Tests.WorldGeneration
{
    [TestFixture()]
    public class WorldGeneratorTests
    {
        /// <summary>
        /// Тест проверяет, что при создании мира не происходит исключений.
        /// </summary>
        /// <returns></returns>
        [Test]
        [Category("longtime")]
        public async Task GenerateAsync_FixedDice_NoExceptions()
        {
            var dice = new Dice(1);
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync();
            //TODO Вынести в отдельное приложение.
            // Суть приложения - генерировать мир и просматривать результат и историю генерации.
            //result.Globe.Save(@"c:\worldgen");

            result.Should().NotBeNull();
        }

        /// <summary>
        /// Временный тест. Нужен для примерного замера производительности на 12000 агентов.
        /// </summary>
        /// <returns></returns>
        [Test]
        [Category("longtime")]
        public async Task GoapTest()
        {
            var _planningManager = new ReGoapPlannerManager<string, object>();
            _planningManager.PlannerSettings = new ReGoap.Planner.ReGoapPlannerSettings
            {
                PlanningEarlyExit = true,
                UsingDynamicActions = true
            };
            _planningManager.Awake();


            var memory = new BuilderMemory();
            memory.Awake();

            var realm = new Realm();
            var agent = new Agent()
            {
                Realm = realm
            };
            var locality = new Locality()
            {
                Owner = realm
            };
            var globe = new Globe()
            {
                Agents = new System.Collections.Generic.List<Agent> { agent },
                Localities = new System.Collections.Generic.List<Locality> { locality }
            };

            // Временное состояние мира.
            // Берём первого попавшегося агента. Потому что на основе этого агента работает goap-агент.
            // Указываем, что в городе, в котором этот агент работает, баланс ресурсов с запасом.
            // Это нужно, чтобы удовлетворить условия действия на строительсво любой структуры.
            var firstAgentLocality = locality;
            foreach (var resource in firstAgentLocality.Stats.Resources)
            {
                memory.GetWorldState().Set($"locality_{firstAgentLocality.Name}_has_{resource.Key}_balance", 1000);
            }

            var goapAgent = new BuilderAgent(memory, globe, agent);



            goapAgent.Awake();


            goapAgent.Start();


            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _planningManager.Update();


            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds + "ms");
        }

        /// <summary>
        /// Временный тест. Нужен для примерного замера производительности на 12000 агентов.
        /// </summary>
        /// <returns></returns>
        [Test]
        [Category("longtime")]
        public async Task GoapTest_Time()
        {
            var _planningManager = new ReGoapPlannerManager<string, object>();
            _planningManager.PlannerSettings = new ReGoap.Planner.ReGoapPlannerSettings
            {
                PlanningEarlyExit = true,
                UsingDynamicActions = true
            };
            _planningManager.Awake();

            for (var i = 0; i < 15000; i++)
            {
                var memory = new BuilderMemory();
                memory.Awake();

                var realm = new Realm();
                var agent = new Agent()
                {
                    Realm = realm
                };
                var locality = new Locality()
                {
                    Owner = realm
                };
                var globe = new Globe()
                {
                    Agents = new System.Collections.Generic.List<Agent> { agent },
                    Localities = new System.Collections.Generic.List<Locality> { locality }
                };

                // Временное состояние мира.
                // Берём первого попавшегося агента. Потому что на основе этого агента работает goap-агент.
                // Указываем, что в городе, в котором этот агент работает, баланс ресурсов с запасом.
                // Это нужно, чтобы удовлетворить условия действия на строительсво любой структуры.
                var firstAgentLocality = locality;
                foreach (var resource in firstAgentLocality.Stats.Resources)
                {
                    memory.GetWorldState().Set($"locality_{firstAgentLocality.Name}_has_{resource.Key}_balance", 1000);
                }

                var goapAgent = new BuilderAgent(memory, globe, agent);



                goapAgent.Awake();


                goapAgent.Start();


            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (var i = 0; i < 40000; i++)
            {
                _planningManager.Update();
            }

            Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds + "ms");
        }

        [Ignore("Эти тесты для ручной проверки. Нужно их привести к нормальным модульным тестам.")]
        [Test]
        public async Task GenerateAsync_ShowHistory()
        {
            var dice = new Dice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync();

            var historyText = string.Empty;

            var iterationHistory = result.History.Items.GroupBy(x => x.Iteration).OrderBy(x => x.Key);

            foreach (var iterationHistoryGroup in iterationHistory)
            {
                historyText += $"{iterationHistoryGroup.Key} iteration" + Environment.NewLine;
                foreach (var historyItem in iterationHistoryGroup)
                {
                    historyText += historyItem.Event + Environment.NewLine;
                }
            }

            Console.WriteLine(historyText);

        }

        [Ignore("Эти тесты для ручной проверки. Нужно их привести к нормальным модульным тестам.")]
        [Test()]
        public async Task GenerateRegionAsyncTest()
        {
            var dice = new Dice();
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync();

            var region = generator.GenerateRegionAsync(result.Globe, result.Globe.Localities.First().Cell);
        }

        [Test]
        public async Task GenerateRegionAsync_StartProvince_RegionHasStartNode()
        {
            // ARRANGE
            var dice = new Dice(1);  // Для тестов указываем кость с фиксированным зурном рандома.
            var schemeService = CreateSchemeService();
            var generator = new WorldGenerator(dice, schemeService);

            var result = await generator.GenerateGlobeAsync();


            // ACT
            var region = await generator.GenerateRegionAsync(result.Globe, result.Globe.StartProvince);



            // ASSERT
            region.RegionNodes.Should().ContainSingle(x => x.IsStart, "В стартовой провинции должен быть один стартовый узел.");
        }

        private ISchemeService CreateSchemeService()
        {
            var schemePath = ConfigurationManager.AppSettings["SchemeCatalog"];

            var schemeLocator = new FileSchemeLocator(schemePath);

            var schemeHandlerFactory = new SchemeServiceHandlerFactory(schemeLocator);

            return new SchemeService(schemeHandlerFactory);
        }
    }
}