﻿using System;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Persons
{
    public static class TacticalActExtensions
    {
        /// <summary>
        /// Проверяет, допустимая ли дистанция для совершения действия.
        /// </summary>
        /// <param name="act"> Проверяемое действие. </param>
        /// <param name="currentNode"> Узел, из которого совершается действие. </param>
        /// <param name="targetNode"> Целевой узел. </param>
        /// <returns>Возвращает true, если дистанция допустима.</returns>
        public static bool CheckDistance(this ICombatAct act,
            IGraphNode currentNode,
            IGraphNode targetNode,
            ISectorMap map)
        {
            if (act is null)
            {
                throw new ArgumentNullException(nameof(act));
            }

            if (currentNode is null)
            {
                throw new ArgumentNullException(nameof(currentNode));
            }

            if (targetNode is null)
            {
                throw new ArgumentNullException(nameof(targetNode));
            }

            if (map is null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            var range = act.Stats.Range;
            if (range is null)
            {
                throw new ArgumentNullException(nameof(act.Stats.Range));
            }

            var distance = map.DistanceBetween(currentNode, targetNode);
            var isInDistance = range.Contains(distance);
            return isInDistance;
        }
    }
}