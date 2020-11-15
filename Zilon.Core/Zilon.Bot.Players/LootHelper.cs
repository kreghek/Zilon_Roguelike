using Zilon.Core.Graphs;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players
{
    public static class LootHelper
    {
        public static IEnumerable<IStaticObject> FindAvailableContainers(
            IEnumerable<IStaticObject> containers,
            IGraphNode observeNode,
            ISectorMap map)
        {
            if (containers is null)
            {
                throw new System.ArgumentNullException(nameof(containers));
            }

            foreach (var container in containers)
            {
                // Проверяем необходимость проверки контейнера
                var props = container.GetModule<IPropContainer>().Content.CalcActualItems();
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