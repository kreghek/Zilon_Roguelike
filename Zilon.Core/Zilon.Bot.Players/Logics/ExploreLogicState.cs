﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Zilon.Core.Graphs;
using Zilon.Core.PersonModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    //TODO Перепроверить работу этого состояния.
    // Есть подозрение, что оно не работает.
    public sealed class ExploreLogicState : MoveLogicStateBase
    {
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public ExploreLogicState(IDecisionSource decisionSource) : base(decisionSource)
        {
        }

        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context,
            ILogicStrategyData strategyData)
        {
            if (MoveTask == null)
            {
                MoveTask = CreateBypassMoveTask(actor, strategyData, context.Sector);

                if (MoveTask != null)
                {
                    return MoveTask;
                }

                // Это может произойти, если актёр не выбрал следующий узел.
                // Тогда переводим актёра в режим ожидания.

                var taskContext = new ActorTaskContext(context.Sector);
                IdleTask = new IdleTask(actor, taskContext, DecisionSource);
                return IdleTask;
            }

            if (!MoveTask.IsComplete)
            {
                // Если команда на перемещение к целевой точке патруля не закончена,
                // тогда продолжаем её.
                // Предварительно проверяем, не мешает ли что-либо её продолжить выполнять.
                if (!MoveTask.CanExecute())
                {
                    MoveTask = CreateBypassMoveTask(actor, strategyData, context.Sector);
                }

                if (MoveTask != null)
                {
                    return MoveTask;
                }

                var taskContext = new ActorTaskContext(context.Sector);
                IdleTask = new IdleTask(actor, taskContext, DecisionSource);
                return IdleTask;
            }

            Complete = true;
            return null;
        }

        private MoveTask CreateBypassMoveTask(IActor actor, ILogicStrategyData strategyData, ISector sector)
        {
            var map = sector.Map;

            var frontNodes = WriteObservedNodesOrGetFromFow(actor, strategyData, sector).ToArray();
            if (!frontNodes.Any())
            {
                return null;
            }

            var availableNodesArray = frontNodes as HexNode[] ?? frontNodes.ToArray();

            if (availableNodesArray.Length == 0)
            {
                // There is no nodes available to roaming.
                // We can do nothing.
                return null;
            }

            for (var i = 0; i < 3; i++)
            {
                var targetNode = DecisionSource.SelectTargetRoamingNode(availableNodesArray);

                if (map.IsPositionAvailableFor(targetNode, actor))
                {
                    var context = new ActorTaskContext(sector);
                    var moveTask = new MoveTask(actor, context, targetNode, map);

                    return moveTask;
                }
            }

            return null;
        }

        private static IGraphNode[] GetKnownNodes(ISector sector, IFowData fowModule)
        {
            var fowItems = fowModule.GetSectorFowData(sector);

            var exploredFowItems = fowItems.GetFowNodeByState(SectorMapNodeFowState.Explored);
            var observingFowItems = fowItems.GetFowNodeByState(SectorMapNodeFowState.Observing);

            var knownNodes = exploredFowItems.Select(x => x.Node).Union(observingFowItems.Select(x => x.Node))
                .ToArray();
            return knownNodes;
        }

        private static IEnumerable<IGraphNode> GetNodesUsingFowModule(
            ILogicStrategyData strategyData,
            ISector sector,
            IFowData fowModule)
        {
            IEnumerable<IGraphNode> frontNodes;

            var knownNodes = GetKnownNodes(sector, fowModule);

            var map = sector.Map;
            var frontNodesHashSet = new HashSet<IGraphNode>();
            foreach (var node in knownNodes)
            {
                // Примечаем выходы
                if (map.Transitions.ContainsKey(node))
                {
                    strategyData.ExitNodes.Add(node);
                }

                var nextUnknownNodes = map.GetNext(node).Where(x => !knownNodes.Contains(x));
                if (nextUnknownNodes.Any())
                {
                    foreach (var unknownNode in nextUnknownNodes)
                    {
                        frontNodesHashSet.Add(unknownNode);
                    }
                }
            }

            frontNodes = frontNodesHashSet;
            return frontNodes;
        }

        private static IEnumerable<IGraphNode> GetNodesUsingStrategyData(
            IActor actor,
            ILogicStrategyData strategyData,
            ISector sector)
        {
            IEnumerable<IGraphNode> frontNodes;
            var map = sector.Map;

            var observeNodes = map.Nodes.Where(x => map.DistanceBetween(x, actor.Node) < 5);

            foreach (var mapNode in observeNodes)
            {
                strategyData.ObservedNodes.Add(mapNode);
            }

            // Собираем пограничные неисследованные узлы.
            var frontNodesHashSet = new HashSet<IGraphNode>();
            foreach (var observedNode in strategyData.ObservedNodes)
            {
                var nextNodes = map.GetNext(observedNode);

                var notObservedNextNodes = nextNodes.Where(x => !strategyData.ObservedNodes.Contains(x));

                foreach (var edgeNode in notObservedNextNodes)
                {
                    frontNodesHashSet.Add(edgeNode);
                }

                // Примечаем выходы
                if (map.Transitions.ContainsKey(observedNode))
                {
                    strategyData.ExitNodes.Add(observedNode);
                }
            }

            var emptyFrontNodes = !frontNodesHashSet.Any();
            var allNodesObserved = map.Nodes.All(x => strategyData.ObservedNodes.Contains(x));

            Debug.Assert((emptyFrontNodes && allNodesObserved) || !emptyFrontNodes,
                "Это состояние выполняется, только если есть неисследованые узлы.");

            frontNodes = frontNodesHashSet;
            return frontNodes;
        }

        private static IEnumerable<IGraphNode> WriteObservedNodesOrGetFromFow(IActor actor,
            ILogicStrategyData strategyData,
            ISector sector)
        {
            IEnumerable<IGraphNode> frontNodes;

            var fowModule = actor.Person.GetModuleSafe<IFowData>();
            if (fowModule is null)
            {
                frontNodes = GetNodesUsingStrategyData(actor, strategyData, sector);
            }
            else
            {
                frontNodes = GetNodesUsingFowModule(strategyData, sector, fowModule);
            }

            if (!frontNodes.Any())
            {
                var closestNode = sector.Map.GetNext(actor.Node).FirstOrDefault();
                if (closestNode != null)
                {
                    frontNodes = new[] { closestNode };
                }
            }

            return frontNodes;
        }
    }
}