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
    internal class ChestValidator : ISectorValidator
    {
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

                var isContainer = allContainerNodes.Contains(neighborHex);

                if (!isContainer)
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

        /// <summary>
        /// Проверяем, что сундук не на клетке с выходом.
        /// </summary>
        private static void ValidateTransitionOverlap(ISector sector, IStaticObject container)
        {
            var transitionNodes = sector.Map.Transitions.Keys;
            var chestOnTransitionNode = transitionNodes.Contains(container.Node);
            if (chestOnTransitionNode)
            {
                throw new SectorValidationException();
            }
        }

        public Task Validate(ISector sector, IServiceProvider scopeContainer)
        {
            return Task.Run(() =>
            {
                // Сундуки не должны генерироваться на узлы, которые являются препятствием.
                // Сундуки не должны генерироваться на узлы с выходом.
                var staticObjectManager = sector.StaticObjectManager;
                var allContainers = staticObjectManager.Items;
                var allContainerNodes = allContainers.Select(x => x.Node).ToArray();
                foreach (var container in allContainers)
                {
                    var hex = (HexNode)container.Node;

                    ValidateTransitionOverlap(sector, container);

                    ValidatePassability(hex, sector.Map, allContainerNodes);
                }
            });
        }
    }
}