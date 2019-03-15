using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour.Bots
{
    public class PatrolLogic : AgressiveLogicBase
    {
        private readonly IPatrolRoute _patrolRoute;
       
        private int? _patrolPointIndex;

        public PatrolLogic(IActor actor,
            IPatrolRoute patrolRoute,
            ISectorMap map,
            IActorManager actors,
            IDecisionSource decisionSource,
            ITacticalActUsageService actService): base(actor, map, actors, decisionSource, actService)
        {
            _patrolRoute = patrolRoute;
        }

        protected override void ProcessMovementComplete()
        {
            _patrolPointIndex++;
            if (_patrolPointIndex >= _patrolRoute.Points.Length)
            {
                _patrolPointIndex = 0;
            }
        }

        protected override MoveTask CreateBypassMoveTask()
        {
            MoveTask moveTask;
            // Если ещё не известна целевая точка патруля
            if (_patrolPointIndex == null)
            {
                var currentPatrolPointIndex = CalcCurrentPatrolPointIndex();

                IMapNode nextPatrolPoint;
                if (currentPatrolPointIndex != null)
                {
                    // Актёр уже стоит в одной из точек патруля.
                    nextPatrolPoint = GetNextPatrolPointFromPatrolPoint(currentPatrolPointIndex.Value);
                }
                else
                {
                    // Актёр не на контрольной точке.
                    // Возвращаемся на маршрут патруля.

                    nextPatrolPoint = GetNextPatrolPointFromField();
                }

                moveTask = new MoveTask(Actor, nextPatrolPoint, Map);
            }
            else
            {
                var targetPatrolPoint = _patrolRoute.Points[_patrolPointIndex.Value];

                moveTask = new MoveTask(Actor, targetPatrolPoint, Map);
            }

            return moveTask;
        }

        protected override void ProcessIntruderDetected()
        {
            _patrolPointIndex = null;
        }


        /// <summary>
        /// Рассчёт следующей контрольной точки, если актёр стоит в поле (не на маршруте патруля).
        /// </summary>
        /// <returns> Возвращает узел карты, представляющий следующую контрольную точку патруля. </returns>
        private IMapNode GetNextPatrolPointFromField()
        {
            var actualPatrolPoints = CalcActualRoutePoints();
            var nearbyPatrolPoint = CalcNearbyPatrolPoint(actualPatrolPoints);
            var nextPatrolPoint = nearbyPatrolPoint;
            return nextPatrolPoint;
        }

        /// <summary>
        /// Расчёт следующей контрольной точке патруля из указанной.
        /// </summary>
        /// <param name="currentPatrolPointIndex"> Текущая точка патруля. </param>
        /// <returns> Возвращает узел карты, представляющий следующую контрольную точку патруля. </returns>
        private IMapNode GetNextPatrolPointFromPatrolPoint(int currentPatrolPointIndex)
        {
            _patrolPointIndex = currentPatrolPointIndex + 1;
            if (_patrolPointIndex >= _patrolRoute.Points.Length)
            {
                _patrolPointIndex = 0;
            }

            var nextPatrolPoint = _patrolRoute.Points[_patrolPointIndex.Value];
            return nextPatrolPoint;
        }

        private int? CalcCurrentPatrolPointIndex()
        {
            int? currentIndex = null;
            for (var i = 0; i < _patrolRoute.Points.Length; i++)
            {
                var routeNode = (HexNode)_patrolRoute.Points[i];
                var actorNode = (HexNode)Actor.Node;
                if (!HexNodeHelper.EqualCoordinates(routeNode, actorNode))
                {
                    continue;
                }

                currentIndex = i;
                break;
            }

            return currentIndex;
        }

        /// <summary>
        /// Получение точек патруля, которые можно обходить.
        /// </summary>
        /// <returns> Набор узлов карты. </returns>
        private HexNode[] CalcActualRoutePoints()
        {
            var hexNodes = _patrolRoute.Points.Cast<HexNode>();
            var actorHexNode = (HexNode)Actor.Node;
            var actualRoutePoints = from node in hexNodes
                                    where !HexNodeHelper.EqualCoordinates(node, actorHexNode)
                                    select node;

            return actualRoutePoints.ToArray();
        }

        private IMapNode CalcNearbyPatrolPoint(IEnumerable<HexNode> routePoints)
        {
            var targets = routePoints;
            var node = (HexNode)Actor.Node;
            var nearbyNode = HexNodeHelper.GetNearbyCoordinates(node, targets);

            if (nearbyNode == null)
            {
                throw new InvalidOperationException("Ближайший узел не найден.");
            }

            return nearbyNode;
        }

    }
}