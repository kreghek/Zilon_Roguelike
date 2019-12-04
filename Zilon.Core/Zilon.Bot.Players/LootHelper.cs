using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Graphs;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players
{
    public static class LootHelper
    {
        public static IEnumerable<IPropContainer> FindAvailableContainers(IEnumerable<IPropContainer> containers,
            IGraphNode observeNode, ISectorMap map)
        {
            foreach (var container in containers)
            {
                // Проверяем необходимость проверки контейнера
                var props = container.Content.CalcActualItems();
                if (!props.Any())
                {
                    continue;
                }

                // Проверяем доступность контейнера для достижения
                var target = container;
                var isVisible = LogicHelper.CheckTargetVisible(map, observeNode, target.Node);
                if (!isVisible)
                {
                    continue;
                }

                yield return container;
            }
        }
    }
}
