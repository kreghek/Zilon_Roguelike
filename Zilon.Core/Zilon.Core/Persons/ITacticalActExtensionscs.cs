using Zilon.Core.Common;

namespace Zilon.Core.Persons
{
    public static class ITacticalActExtensionscs
    {
        /// <summary>
        /// Проверяет, допустимая ли дистанция для совершения действия.
        /// </summary>
        /// <param name="act"> Преверяемое действие. </param>
        /// <param name="currentCubePos"> Узел, из которого совершается действие. </param>
        /// <param name="targetCubePos"> Целевой узел. </param>
        /// <returns>Возвращает true, если дистанция допустима.</returns>
        public static bool CheckDistance(this ITacticalAct act,
            CubeCoords currentCubePos,
            CubeCoords targetCubePos)
        {
            var range = new Range<int>(act.Scheme.MinRange, act.Scheme.MaxRange);
            var distance = currentCubePos.DistanceTo(targetCubePos);
            var isInDistance = range.Contains(distance);
            return isInDistance;
        }
    }
}
