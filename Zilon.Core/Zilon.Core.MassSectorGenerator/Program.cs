using System;
using System.Linq;

using LightInject;

using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MassSectorGenerator
{
    class Program
    {
        static async System.Threading.Tasks.Task Main()
        {
            var startUp = new Startup();
            var serviceContainer = new ServiceContainer();
            serviceContainer.EnableAnnotatedConstructorInjection();
            startUp.RegisterServices(serviceContainer);


            var schemeService = serviceContainer.GetInstance<ISchemeService>();
            var allLocations = schemeService.GetSchemes<ILocationScheme>()
                .Where(x => x.SectorLevels != null).ToArray();

            var random = new Random();
            var iteration = 0;
            while (true)
            {
                var schemeCount = allLocations.Length;
                var randomSchemeIndex = random.Next(0, schemeCount);
                var sectorScheme = allLocations[randomSchemeIndex];
                var sectorLevelIndex = random.Next(0, sectorScheme.SectorLevels.Length);
                var sectorLevel = sectorScheme.SectorLevels[sectorLevelIndex];

                iteration++;

                using (var scopeContainer = serviceContainer.BeginScope())
                {
                    var sectorFactory = scopeContainer.GetInstance<ISectorGenerator>();
                    var sector = await sectorFactory.GenerateDungeonAsync(sectorLevel);
                    sector.Scheme = sectorScheme;

                    // Проверка

                    // Проверка сундуков.
                    // Сундуки не должны генерироваться на узлы, которые являются препятствием.
                    // Сундуки не должны генерироваться на узлы с выходом.
                    var containerManager = scopeContainer.GetInstance<IPropContainerManager>();
                    var allContainers = containerManager.Items;
                    foreach (var container in allContainers)
                    {
                        // Проверяем, что сундук не стоит на препятствии.
                        var hex = (HexNode)container.Node;
                        if (hex.IsObstacle)
                        {
                            throw new Exception();
                        }

                        // Проверяем, что сундук не на клетке с выходом.
                        var transitionNodes = sector.Map.Transitions.Keys;
                        var chestOnTransitionNode = transitionNodes.Contains(container.Node);
                        if (chestOnTransitionNode)
                        {
                            throw new Exception();
                        }
                    }

                    // Проверка монстров.
                    // Монстры не должны генерироваться на узлах с препятствием.
                    // Монстры не должны генерироваться на узлах с сундуками.
                    var actorManager = scopeContainer.GetInstance<IActorManager>();
                    var allMonsters = actorManager.Items;
                    var containerNodes = allContainers.Select(x => x.Node);
                    foreach (var actor in allMonsters)
                    {
                        var hex = (HexNode)actor.Node;
                        if (hex.IsObstacle)
                        {
                            throw new Exception();
                        }

                        var monsterIsOnContainer = containerNodes.Contains(actor.Node);
                        if (monsterIsOnContainer)
                        {
                            throw new Exception();
                        }
                    }

                    // Проверка переходов.

                    var transitions = sector.Map.Transitions.Values;

                    // В секторе должны быть выходы.

                    if (transitions.Any())
                    {
                        var hasStartRegion = false;
                        var hasTransitionInregionsNodes = false;
                        foreach (var region in sector.Map.Regions)
                        {
                            if (region.IsStart)
                            {
                                hasStartRegion = true;
                            }

                            if ((region.ExitNodes?.Any()).GetValueOrDefault())
                            {
                                hasTransitionInregionsNodes = true;
                            }
                        }

                        if (!hasStartRegion)
                        {
                            // Хоть один регион должен быть отмечен, как стартовый.
                            // Чтобы клиенты знали, где размещать персонажа после генерации.
                            throw new Exception($"Не задан стартовый регион.");
                        }

                        if (!hasTransitionInregionsNodes)
                        {
                            // Переходы должны быть явно обозначены в регионах.
                            //TODO Рассмотреть вариант упрощения
                            // В секторе уже есть информация об узлах с переходами.
                            // Выглядит, как дублирование.
                            throw new Exception($"Не указан ни один регион с узламы перехода.");
                        }
                    }
                    else
                    {
                        // Если в секторе нет переходов, то будет невозможно его покинуть.
                        throw new Exception("В секторе не найдены переходы.");
                    }

                    // Все переходы на уровне должны либо вести на глобальную карту,
                    // либо на корректный уровень сектора.

                    foreach (var transition in transitions)
                    {
                        var targetSectorSid = transition.SectorSid;
                        if (targetSectorSid == null)
                        {
                            // Это значит, что переход на глобальную карту.
                            // Нормальная ситуация, проверяем следующий переход.
                            continue;
                        }

                        var sectorLevelBySid = sector.Scheme.SectorLevels.SingleOrDefault(level => level.Sid == targetSectorSid);
                        if (sectorLevelBySid == null)
                        {
                            throw new Exception($"Не найден уровень сектора {targetSectorSid}, указанный в переходе.");
                        }
                    }
                }

                Console.WriteLine($"Iteration {iteration:D5} complete");
                Console.WriteLine($"{sectorScheme.Name.En} Level {sectorLevelIndex}");

                if (iteration >= 100)
                {
                    break;
                }
            }
        }
    }
}
