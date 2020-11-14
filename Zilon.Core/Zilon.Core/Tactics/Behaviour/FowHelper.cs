using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Вспомогательный класс для рассчёта тумана войны.
    /// </summary>
    public static class FowHelper
    {
        /// <summary>
        /// Обновление состояния тумана войны для актёра с учётом карты и опорного узла карты.
        /// </summary>
        /// <param name="fowData">Состояние тумана войны которого обновляется.</param>
        /// <param name="fowContext">Контекст тумана войны.</param>
        /// <param name="baseNode">Опорный узел.</param>
        /// <param name="radius">Радиус обзора персонажа.</param>
        public static void UpdateFowData(ISectorFowData fowData, IFowContext fowContext, IGraphNode baseNode,
            int radius)
        {
            if (fowData is null)
            {
                throw new System.ArgumentNullException(nameof(fowData));
            }

            if (fowContext is null)
            {
                throw new System.ArgumentNullException(nameof(fowContext));
            }

            if (baseNode is null)
            {
                throw new System.ArgumentNullException(nameof(baseNode));
            }

            // Все наблюдаемые из базового узла узлы карты.
            var observingNodes = GetObservingNodes(fowContext, baseNode, radius);

            var currentObservedFowNodes = fowData.GetFowNodeByState(SectorMapNodeFowState.Observing);

            var newObservedFowNodes = UpdateOrCreateFowNodes(fowData, observingNodes);

            var notObservingFowNodes = currentObservedFowNodes.Except(newObservedFowNodes).ToArray();

            foreach (var fowNode in notObservingFowNodes)
            {
                fowData.ChangeNodeState(fowNode, SectorMapNodeFowState.Explored);
            }
        }

        private static SectorMapFowNode[] UpdateOrCreateFowNodes(ISectorFowData fowData, IGraphNode[] observingNodes)
        {
            var observedFowNodes = new List<SectorMapFowNode>();

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

                fowData.ChangeNodeState(fowNode, SectorMapNodeFowState.Observing);
                observedFowNodes.Add(fowNode);
            }

            return observedFowNodes.ToArray();
        }

        private static SectorMapFowNode GetFowNodeByMapNode(ISectorFowData fowData, IGraphNode observingNode)
        {
            return fowData.GetNode(observingNode);
        }

        private static IGraphNode[] GetObservingNodes(IFowContext fowContext, IGraphNode baseNode, int radius)
        {
            var border = new List<IGraphNode> { baseNode };

            var resultList = new List<IGraphNode> { baseNode };

            // Шаги заливки
            for (var stepIndex = 1; stepIndex <= radius; stepIndex++)
            {
                var newBorder = GetNextForBorder(border, resultList, fowContext);

                var visibleBorder = newBorder.AsParallel().Where(x => fowContext.IsTargetVisible(x, baseNode))
                    .ToArray();

                border.Clear();
                border.AddRange(visibleBorder);
                resultList.AddRange(visibleBorder);
            }

            return resultList.ToArray();
        }

        private static IGraphNode[] GetNextForBorder(IEnumerable<IGraphNode> border, IEnumerable<IGraphNode> result,
            IFowContext fowContext)
        {
            var borderTotal = new List<IGraphNode>();

            foreach (var node in border)
            {
                var next = fowContext.GetNext(node);

                var except = border.Union(result).Union(borderTotal);

                var newBorder = next.Except(except);

                borderTotal.AddRange(newBorder);
            }

            return borderTotal.ToArray();
        }
    }
}