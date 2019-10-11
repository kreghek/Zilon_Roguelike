using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightInject;

using Zilon.CommonUtilities;
using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tactics.Spatial.PathFinding;

namespace Zilon.Core.MassSectorGenerator
{
    class Program
    {
        private readonly static Random _random;

        static Program()
        {
            _random = new Random();
        }

        static async Task Main(string[] args)
        {
            var diceSeed = GetDiceSeed(args);
            var outputPath = GetOutputPath(args);

            var startUp = new Startup(diceSeed);
            var serviceContainer = new ServiceContainer();
            serviceContainer.EnableAnnotatedConstructorInjection();
            startUp.RegisterServices(serviceContainer);

            var schemeService = serviceContainer.GetInstance<ISchemeService>();

            var sectorSchemeResult = GetSectorScheme(args, schemeService);

            using (var scopeContainer = serviceContainer.BeginScope())
            {
                var sectorFactory = scopeContainer.GetInstance<ISectorGenerator>();
                var sector = await sectorFactory.GenerateDungeonAsync(sectorSchemeResult.Sector);
                sector.Scheme = sectorSchemeResult.Location;

                // Проверка

                var checkTask = CheckSectorAsync(scopeContainer, sector);
                var saveTask = SaveMapAsImageAsync(outputPath, sector);

                await Task.WhenAll(checkTask, saveTask);
            }

            serviceContainer.Dispose();
        }

        private static Task SaveMapAsImageAsync(string outputPath, ISector sector)
        {
            if (outputPath != null)
            {
                SaveMapAsImage(sector.Map, outputPath);
            }

            return Task.CompletedTask;
        }

        private static Task CheckSectorAsync(Scope scopeContainer, ISector sector)
        {
            var checkNodesTask = CheckNodesAsync(sector, scopeContainer);

            var checkChestsTask = CheckChestsAsync(scopeContainer, sector);

            var checkMonstersTask = CheckMonstersAsync(scopeContainer);

            var checkTransitionsTask = CheckTransitionsAsync(sector);

            var allTasks = Task.WhenAll(checkNodesTask, checkChestsTask, checkMonstersTask, checkTransitionsTask);

            try
            {
                allTasks.Wait();
            }
            catch (AggregateException exception)
            {
                Log.Error("Сектор содержит ошибки:");

                foreach (var inner in exception.InnerExceptions)
                {
                    if (inner is SectorValidationException)
                    {
                        Log.Error(inner);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return Task.CompletedTask;
        }

        private static string GetOutputPath(string[] args)
        {
            var outputPath = ArgumentHelper.GetProgramArgument(args, Args.OUT_PATH_ARG_NAME);
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                return null;
            }

            try
            {
                Path.GetFullPath(outputPath);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
                return null;
            }

            return outputPath;
        }

        private static int GetDiceSeed(string[] args)
        {
            var diceSeedString = ArgumentHelper.GetProgramArgument(args, Args.DICE_SEED_ARG_NAME);

            int diceSeed;
            if (string.IsNullOrWhiteSpace(diceSeedString))
            {
                diceSeed = _random.Next(0, int.MaxValue);
            }
            else
            {
                if (!int.TryParse(diceSeedString, out diceSeed))
                {
                    throw new SectorGeneratorException($"Зерно рандома задано некорректно: {diceSeedString}.");
                }
            }

            Log.Info($"DICE SEED: {diceSeed}");

            return diceSeed;
        }

        private static SectorSchemeResult GetSectorScheme(string[] args, ISchemeService schemeService)
        {
            var locationSchemeSid = ArgumentHelper.GetProgramArgument(args, Args.LOCATION_SCHEME_SID_ARG_NAME);
            var sectorSchemeSid = ArgumentHelper.GetProgramArgument(args, Args.SECTOR_SCHEME_SID_ARG_NAME);
            if (string.IsNullOrWhiteSpace(locationSchemeSid) && string.IsNullOrWhiteSpace(sectorSchemeSid))
            {
                // Если схемы не указаны, то берём случайную схему.
                // Это используется на билд-сервере, чтобы случайно проверить несколько схем.

                var locationSchemes = schemeService.GetSchemes<ILocationScheme>()
                    .Where(x => x.SectorLevels != null && x.SectorLevels.Any())
                    .ToArray();
                var locationSchemeIndex = _random.Next(0, locationSchemes.Length);
                var locationScheme = locationSchemes[locationSchemeIndex];

                var sectorSchemes = locationScheme.SectorLevels;
                var sectorSchemeIndex = _random.Next(0, sectorSchemes.Length);
                var sectorScheme = sectorSchemes[sectorSchemeIndex];

                Log.Info($"SCHEME: {locationScheme.Sid} - {sectorScheme.Sid}(index:{sectorSchemeIndex})");

                var result = new SectorSchemeResult(locationScheme, sectorScheme);

                return result;
            }
            else
            {
                // Если схемы заданы, то строим карту на их основе.
                // Это будет использовано для отладки.

                var locationScheme = schemeService.GetScheme<ILocationScheme>(locationSchemeSid);
                if (locationScheme == null)
                {
                    throw new SectorGeneratorException($"Не найдена схема локации {locationSchemeSid}.");
                }

                var sectorScheme = locationScheme.SectorLevels.SingleOrDefault(x => x.Sid == sectorSchemeSid);
                if (sectorScheme == null)
                {
                    throw new SectorGeneratorException($"Не найдена схема сектора {sectorSchemeSid}.");
                }

                var result = new SectorSchemeResult(locationScheme, sectorScheme);

                return result;
            }
        }

        private static Task CheckNodesAsync(ISector sector, Scope scopeContainer)
        {
            // Проверяем проходимость карты.
            // Для этого убеждаемся, что из любого узла есть путь до любого другого.
            // При поиске пути:
            // - Считаем непроходимыме все статические объекты. Это декоратиные препятствия и сундуки.
            // - Игнорируем все перемещаемые. Например, монстров.

            return Task.Run(() =>
            {
                var allNodes = sector.Map.Nodes.OfType<HexNode>().Where(x => !x.IsObstacle).ToArray();
                var containerNodes = scopeContainer.GetInstance<IPropContainerManager>();

                var parallelResult = Parallel.ForEach(allNodes, startNode => {
                    foreach (var goalNode in allNodes)
                    {
                        if (startNode == goalNode)
                        {
                            // Не ищем путь из узла до самого себя.
                            continue;
                        }

                        var astar = new AStarSimpleHex(sector.Map, containerNodes, startNode, goalNode);
                        var result = astar.Run();
                        if (result != State.GoalFound)
                        {
                            throw new SectorValidationException();
                        }
                    }
                });
            });
        }

        private static Task CheckTransitionsAsync(ISector sector)
        {
            return Task.Run(() =>
            {
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
                        throw new SectorValidationException($"Не задан стартовый регион.");
                    }

                    if (!hasTransitionInregionsNodes)
                    {
                        // Переходы должны быть явно обозначены в регионах.
                        //TODO Рассмотреть вариант упрощения
                        // В секторе уже есть информация об узлах с переходами.
                        // Выглядит, как дублирование.
                        throw new SectorValidationException($"Не указан ни один регион с узламы перехода.");
                    }
                }
                else
                {
                    // Если в секторе нет переходов, то будет невозможно его покинуть.
                    throw new SectorValidationException("В секторе не найдены переходы.");
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
                        throw new SectorValidationException($"Не найден уровень сектора {targetSectorSid}, указанный в переходе.");
                    }
                }
            });
        }

        private static Task CheckMonstersAsync(Scope scopeContainer)
        {
            return Task.Run(() =>
            {
                var containerManager = scopeContainer.GetInstance<IPropContainerManager>();
                var allContainers = containerManager.Items;

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
                        throw new SectorValidationException();
                    }

                    var monsterIsOnContainer = containerNodes.Contains(actor.Node);
                    if (monsterIsOnContainer)
                    {
                        throw new SectorValidationException();
                    }
                }
            });
        }

        private static Task CheckChestsAsync(Scope scopeContainer, ISector sector)
        {
            return Task.Run(() =>
            {
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
                        throw new SectorValidationException();
                    }

                    // Проверяем, что сундук не на клетке с выходом.
                    var transitionNodes = sector.Map.Transitions.Keys;
                    var chestOnTransitionNode = transitionNodes.Contains(container.Node);
                    if (chestOnTransitionNode)
                    {
                        throw new SectorValidationException();
                    }
                }
            });
        }

        private static void SaveMapAsImage(ISectorMap map, string outputPath)
        {
            var bmp = MapDrawer.DrawMap(map);

            bmp.Save(outputPath);
        }
    }
}
