using System.Linq;
using System.Threading.Tasks;

using LightInject;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tactics.Spatial.PathFinding;

namespace Zilon.Core.MassSectorGenerator.SectorValidators
{
    class PassMapValidator : ISectorValidator
    {
        public Task Validate(ISector sector, Scope scopeContainer)
        {
            // Проверяем проходимость карты.
            // Для этого убеждаемся, что из любого узла есть путь до любого другого.
            // При поиске пути:
            // - Считаем непроходимыме все статические объекты. Это декоратиные препятствия и сундуки.
            // - Игнорируем все перемещаемые. Например, монстров.

            return Task.Run(() =>
            {
                var containerManager = scopeContainer.GetInstance<IPropContainerManager>();
                var containerNodes = containerManager.Items.Select(x => x.Node);

                var allNonObstacleNodes = sector.Map.Nodes.OfType<HexNode>().Where(x => !x.IsObstacle).ToArray();
                var allNonContainerNodes = allNonObstacleNodes.Where(x => !containerNodes.Contains(x));
                var allNodes = allNonContainerNodes.ToArray();

                var parallelResult = Parallel.ForEach(allNodes, startNode =>
                {
                    foreach (var goalNode in allNodes)
                    {
                        if (startNode == goalNode)
                        {
                            // Не ищем путь из узла до самого себя.
                            continue;
                        }

                        var astar = new AStarSimpleHex(sector.Map, containerManager, startNode, goalNode);
                        var result = astar.Run();
                        if (result != State.GoalFound)
                        {
                            throw new SectorValidationException();
                        }
                    }
                });
            });
        }
    }
}
