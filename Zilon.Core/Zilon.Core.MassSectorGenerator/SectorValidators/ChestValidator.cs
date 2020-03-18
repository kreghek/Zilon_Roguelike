using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MassSectorGenerator.SectorValidators
{
    /// <summary>
    /// Валидатор контейнеров в секторе.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance",
        "CA1812:Avoid uninstantiated internal classes",
        Justification = "Регистрируется в контейнере зависимостей через рефлексию.")]
    class ChestValidator : ISectorValidator
    {
        public Task Validate(ISector sector, IServiceProvider scopeContainer)
        {
            return Task.Run(() =>
            {
                // Сундуки не должны генерироваться на узлы, которые являются препятствием.
                // Сундуки не должны генерироваться на узлы с выходом.
                var containerManager = scopeContainer.GetRequiredService<IPropContainerManager>();
                var allContainers = containerManager.Items;
                var allContainerNodes = allContainers.Select(x => x.Node).ToArray();
                foreach (var container in allContainers)
                {
                    var hex = (HexNode)container.Node;

                    ValidateObstacleOverlap(hex);

                    ValidateTransitionOverlap(sector, container);

                    ValidatePassability(hex, sector.Map, allContainerNodes);
                }
            });
        }

        /// <summary>
        /// Проверяем, что сундук не на клетке с выходом.
        /// </summary>
        private static void ValidateTransitionOverlap(ISector sector, IPropContainer container)
        {
            var transitionNodes = sector.Map.Transitions.Keys;
            var chestOnTransitionNode = transitionNodes.Contains(container.Node);
            if (chestOnTransitionNode)
            {
                throw new SectorValidationException();
            }
        }

        /// <summary>
        /// Проверяем, что сундук не стоит на препятствии.
        /// </summary>
        private static void ValidateObstacleOverlap(HexNode hex)
        {
            if (hex.IsObstacle)
            {
                throw new SectorValidationException();
            }
        }

        /// <summary>
        /// Проверяем, что к сундуку есть подход.
        /// </summary>
        private static void ValidatePassability(
            HexNode currentContainerHex,
            ISectorMap sectorMap,
            IGraphNode[] allContainerNodes)
        {
            var neighborNodes = sectorMap.GetNext(currentContainerHex);
            var hasFreeNeighbor = false;
            foreach (var neighborNode in neighborNodes)
            {
                var neighborHex = (HexNode)neighborNode;

                var isObstacle = neighborHex.IsObstacle;
                var isContainer = allContainerNodes.Contains(neighborHex);

                if (!isObstacle && !isContainer)
                {
                    hasFreeNeighbor = true;
                    break;
                }
            }

            if (!hasFreeNeighbor)
            {
                throw new SectorValidationException($"Контейнер {currentContainerHex} не имеет подступов.");
            }
        }
    }
}
