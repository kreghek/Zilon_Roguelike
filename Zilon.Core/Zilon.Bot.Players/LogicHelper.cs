using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players
{
    public static class LogicHelper
    {
        private const int VISIBLE_RANGE = 5;

        public static bool CheckTargetVisible(ISectorMap map, IMapNode node, IMapNode target)
        {
            var distance = map.DistanceBetween(node, target);

            var isVisible = distance <= VISIBLE_RANGE;
            return isVisible;
        }
    }
}
