using System.Linq;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public static class FowHelper
    {
        /// <summary>
        /// Обновление состояния тумана войны для актёра с учётом карты и опорного узла карты.
        /// </summary>
        /// <param name="fowData">Состояние тумана войны которого обновляется.</param>
        /// <param name="map">Карта, на которой действует актёр.</param>
        /// <param name="baseNode">Опорный узел.</param>
        /// <param name="radius">Радиус обзора персонажа.</param>
        public static void UpdateFowData(ISectorFowData fowData, ISectorMap map, IGraphNode baseNode, int radius)
        {
            if (fowData is null)
            {
                throw new System.ArgumentNullException(nameof(fowData));
            }

            if (map is null)
            {
                throw new System.ArgumentNullException(nameof(map));
            }

            if (baseNode is null)
            {
                throw new System.ArgumentNullException(nameof(baseNode));
            }

            // Все наблюдаемые из базового узла узлы карты.
            var observingNodes = GetObservingNodes(map, baseNode, radius);
            UpdateOrCreateFowNodes(fowData, observingNodes);
        }

        private static void UpdateOrCreateFowNodes(ISectorFowData fowData, IGraphNode[] observingNodes)
        {
            foreach (var observingNode in observingNodes)
            {
                // Если узла нет в данных о тумане войны, то добавляем его.
                // Текущий узел в тумане войны переводим в состояние наблюдаемого.
                var fowNode = GetFowNodeByMapNode(fowData, observingNode);

                if (fowNode == null)
                {
                    fowNode = new SectorMapFowNode(observingNode);
                    fowData.AddNodes(new[] { fowNode });
                }

                fowNode.ChangeState(SectorMapNodeFowState.Observing);
            }
        }

        private static SectorMapFowNode GetFowNodeByMapNode(ISectorFowData fowData, IGraphNode observingNode)
        {
            return fowData.GetNode(observingNode);
        }

        private static IGraphNode[] GetObservingNodes(ISectorMap map, IGraphNode baseNode, int radius)
        {
            //TODO Оптимизировать
            return map.Nodes.Where(x => map.DistanceBetween(x, baseNode) <= radius && map.TargetIsOnLine(x, baseNode)).ToArray();
        }
    }
}
