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
        /// <param name="actor">Актёр, состояние тумана войны которого обновляется.</param>
        /// <param name="map">Карта, на которой действует актёр.</param>
        /// <param name="baseNode">Опорный узел.</param>
        /// <param name="radius">Радиус обзора персонажа.</param>
        public static void UpdateFowData(IActor actor, ISectorMap map, IGraphNode baseNode, int radius)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
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
            var observingNodes = map.Nodes.Where(x => map.DistanceBetween(x, baseNode) <= radius).ToArray();

            foreach (var observingNode in observingNodes)
            {
                // Если узла нет в данных о тумане войны, то добавляем его.
                // Текущий узел в тумане войны переводим в состояние наблюдаемого.
                var fowNode = actor.SectorFowData.Nodes.SingleOrDefault(x => x.Node == observingNode);

                if (fowNode == null)
                {
                    fowNode = new SectorMapFowNode(observingNode);
                    actor.SectorFowData.AddNodes(new[] { fowNode });
                }

                //fowNode.ChangeState(SectorMapNodeFowState.Observing);
            }
        }
    }
}
