using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players
{
    public static class LogicHelper
    {
        /// <summary>
        /// Радиус обхора персонажа.
        /// </summary>
        private const int SIGN_RANGE = 5;

        public static bool CheckTargetVisible(ISectorMap map, IGraphNode node, IGraphNode target)
        {
            if (map is null)
            {
                throw new System.ArgumentNullException(nameof(map));
            }

            var distance = map.DistanceBetween(node, target);
            var isInSignRange = distance <= SIGN_RANGE;
            if (!isInSignRange)
            {
                return false;
            }

            var isVisible = map.TargetIsOnLine(node, target);
            if (!isVisible)
            {
                return false;
            }

            return true;
        }
    }
}