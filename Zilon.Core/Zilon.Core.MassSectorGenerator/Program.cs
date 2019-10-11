using System;
using System.Linq;

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

        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var diceSeed = GetDiceSeed(args);

            var startUp = new Startup(diceSeed);
            var serviceContainer = new ServiceContainer();
            serviceContainer.EnableAnnotatedConstructorInjection();
            startUp.RegisterServices(serviceContainer);

            var schemeService = serviceContainer.GetInstance<ISchemeService>();

            var sectorScheme = GetSectorScheme(args, schemeService);

            using (var scopeContainer = serviceContainer.BeginScope())
            {
                var sectorFactory = scopeContainer.GetInstance<ISectorGenerator>();
                var sector = await sectorFactory.GenerateDungeonAsync(sectorScheme);
                sector.Scheme = sectorScheme;

                // Проверка

                CheckNodes(sector, scopeContainer);

                CheckChests(scopeContainer, sector);

                CheckMonsters(scopeContainer);

                CheckTransitions(sector);
            }



            serviceContainer.Dispose();
        }

        private static int GetDiceSeed(string[] args)
        {
            var diceSeedString = ArgumentHelper.GetProgramArgument(args, Args.DICE_SEED_ARG_NAME);

            if (!int.TryParse(diceSeedString, out var diceSeed))
            {
                throw new SectorGeneratorException($"Зерно рандома задано некорректно: {diceSeedString}.");
            }

            return diceSeed;
        }

        private static ISectorSubScheme GetSectorScheme(string[] args, ISchemeService schemeService)
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

                return sectorScheme;
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

                return sectorScheme;
            }
        }

        private static void CheckNodes(ISector sector, Scope scopeContainer)
        {
            // Проверяем проходимость карты.
            // Для этого убеждаемся, что из любого узла есть путь до любого другого.
            // При поиске пути:
            // - Считаем непроходимыме все статические объекты. Это декоратиные препятствия и сундуки.
            // - Игнорируем все перемещаемые. Например, монстров.

            var allNodes = sector.Map.Nodes.OfType<HexNode>().Where(x => !x.IsObstacle).ToArray();
            var containerNodes = scopeContainer.GetInstance<IPropContainerManager>();

            foreach (var startNode in allNodes)
            {
                foreach (var endNode in allNodes)
                {
                    if (startNode == endNode)
                    {
                        // Не ищем путь из узла до самого себя.
                        continue;
                    }

                    var astar = new AStarSimpleHex(sector.Map, containerNodes, startNode, endNode);
                    var result = astar.Run();
                    if (result != State.GoalFound)
                    {
                        throw new Exception();
                    }
                }
            }
        }

        private static void CheckTransitions(ISector sector)
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

        private static void CheckMonsters(Scope scopeContainer)
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
                    throw new Exception();
                }

                var monsterIsOnContainer = containerNodes.Contains(actor.Node);
                if (monsterIsOnContainer)
                {
                    throw new Exception();
                }
            }
        }

        private static void CheckChests(Scope scopeContainer, ISector sector)
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
        }
    }
}
