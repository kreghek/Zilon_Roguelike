using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Common;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MassSectorGenerator.SectorValidators
{
    /// <summary>
    /// Валидатор проходимости карты сектора.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance",
        "CA1812:Avoid uninstantiated internal classes",
        Justification = "Регистрируется в контейнере зависимостей через рефлексию.")]
    class PassMapValidator : ISectorValidator
    {
        public Task Validate(ISector sector, IServiceProvider scopeContainer)
        {
            // Проверяем проходимость карты.
            // Для этого убеждаемся, что из любого узла есть путь до любого другого.
            // При поиске пути:
            // - Считаем непроходимыме все статические объекты. Это декоратиные препятствия и сундуки.
            // - Игнорируем все перемещаемые. Например, монстров.

            return Task.Run(() =>
            {
                var containerManager = scopeContainer.GetRequiredService<IPropContainerManager>();
                var containerNodes = containerManager.Items.Select(x => x.Node);

                var allNonObstacleNodes = sector.Map.Nodes.OfType<HexNode>().Where(x => !x.IsObstacle).ToArray();
                var allNonContainerNodes = allNonObstacleNodes.Where(x => !containerNodes.Contains(x));
                var allNodes = allNonContainerNodes.ToArray();

                var matrix = new Matrix<bool>(1000, 1000);
                foreach (var node in allNodes)
                {
                    var x = node.OffsetX;
                    var y = node.OffsetY;
                    matrix.Items[x, y] = true;
                }

                var startNode = allNodes.First();
                var startPoint = new OffsetCoords(startNode.OffsetX, startNode.OffsetY);
                var floodPoints = HexBinaryFiller.FloodFill(matrix, startPoint);

                foreach (var point in floodPoints)
                {
                    matrix.Items[point.X, point.Y] = false;
                }

                foreach (var node in allNodes)
                {
                    var x = node.OffsetX;
                    var y = node.OffsetY;
                    if (matrix.Items[x, y])
                    {
                        throw new SectorValidationException($"Точка ({x}, {y}) недоступна для прохода.");
                    }
                }
            });
        }
    }
}
