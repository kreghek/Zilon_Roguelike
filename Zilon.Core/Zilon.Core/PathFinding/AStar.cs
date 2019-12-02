using System.Collections.Generic;

using Zilon.Core.Graphs;

namespace Zilon.Core.PathFinding
{
    /// <summary>
    /// Interface to setup and run the AStar algorithm.
    /// </summary>
    /// <remarks>
    /// https://ru.wikipedia.org/wiki/A*
    /// Общий алгоритм такой:
    /// 1. В начале в список открытых узлов помещается стартовый узел.
    /// 2. Из открытого списка выбирается первый узел, которого нет в списке закрытых.
    /// Первый выбранный будет наиболее дешёвый, т.к. открытый список сортируется.
    /// 3. Получаем всех соседей узла и размещаем из в открытый список. Соседи должны отсутствовать в закрытом списке.
    /// 4. Для каждого соседа запоминаем, как мы к нему пришли.
    /// 5. В конце по отметаким, как пришли восстанавливаем весь путь.
    /// </remarks>
    public sealed class AStar
    {
        /// <summary>
        /// The open list.
        /// </summary>
        private readonly SortedList<int, IGraphNode> _openList;

        /// <summary>
        /// The closed list.
        /// </summary>
        private readonly HashSet<IGraphNode> _closedList;

        private readonly IAstarContext _context;
        private readonly Dictionary<IGraphNode, AStarData> _dataDict;

        /// <summary>
        /// The goal node.
        /// </summary>
        private IGraphNode _goal;

        /// <summary>
        /// Gets the current node that the AStar algorithm is at.
        /// </summary>
        public IGraphNode CurrentNode { get; private set; }

        /// <summary>
        /// Creates a new AStar algorithm instance with the provided start and goal nodes.
        /// </summary>
        /// <param name="context"> Контекст выполнения поиска (способности персонажа, служебная информация). </param>
        /// <param name="start">The starting node for the AStar algorithm.</param>
        /// <param name="goal">The goal node for the AStar algorithm.</param>
        public AStar(IAstarContext context, IGraphNode start, IGraphNode goal)
        {
            if (start == null)
            {
                throw new System.ArgumentNullException(nameof(start));
            }

            if (goal == null)
            {
                throw new System.ArgumentNullException(nameof(goal));
            }

            _openList = new SortedList<int, IGraphNode>(DuplicateComparer.Instance);
            _closedList = new HashSet<IGraphNode>();
            _dataDict = new Dictionary<IGraphNode, AStarData>();

            _context = context ?? throw new System.ArgumentNullException(nameof(context));

            Reset(start, goal);
        }

        /// <summary>
        /// Resets the AStar algorithm with the newly specified start node and goal node.
        /// </summary>
        /// <param name="start">The starting node for the AStar algorithm.</param>
        /// <param name="goal">The goal node for the AStar algorithm.</param>
        private void Reset(IGraphNode start, IGraphNode goal)
        {
            _openList.Clear();
            _closedList.Clear();
            _dataDict.Clear();

            CurrentNode = start;
            _goal = goal;

            var currentData = GetData(CurrentNode);
            _openList.AddWithData(CurrentNode, currentData);

        }

        /// <summary>
        /// Steps the AStar algorithm forward until it either fails or finds the goal node.
        /// </summary>
        /// <returns>Returns the state the algorithm finished in, Failed or GoalFound.</returns>
        public State Run()
        {
            // Continue searching until either failure or the goal node has been found.
            while (true)
            {
                var state = Step();
                if (state != State.Searching)
                {
                    return state;
                }
            }
        }

        /// <summary>
        /// Moves the AStar algorithm forward one step.
        /// </summary>
        /// <returns>Returns the state the alorithm is in after the step, either Failed, GoalFound or still Searching.</returns>
        private State Step()
        {
            while (true)
            {
                // There are no more nodes to search, return failure.
                if (_openList.IsEmpty())
                {
                    return State.Failed;
                }

                // Check the next best node in the graph by TotalCost.
                CurrentNode = _openList.Pop();

                // This node has already been searched, check the next one.
                if (_closedList.Contains(CurrentNode))
                {
                    continue;
                }

                // An unsearched node has been found, search it.
                break;
            }

            // Remove from the open list and place on the closed list 
            // since this node is now being searched.

            var currentData = GetData(CurrentNode);

            _openList.Remove(currentData.TotalCost);

            _closedList.Add(CurrentNode);

            // Found the goal, stop searching.
            if (CurrentNode == _goal)
            {
                return State.GoalFound;
            }

            // Node was not the goal so add all children nodes to the open list.
            // Each child needs to have its movement cost set and estimated cost.

            var neighbors = _context.GetNext(CurrentNode);

            foreach (var child in neighbors)
            {
                // If the child has already been searched (closed list) or is on
                // the open list to be searched then do not modify its movement cost
                // or estimated cost since they have already been set previously.
                if (_openList.ContainsValue(child) || _closedList.Contains(child))
                {
                    continue;
                }

                var childData = GetData(child);
                currentData = GetData(CurrentNode);

                childData.Parent = CurrentNode;
                childData.MovementCost = currentData.MovementCost + 1;
                childData.EstimateCost = _context.GetDistanceBetween(CurrentNode, _goal);

                _openList.AddWithData(child, childData);
            }

            // This step did not find the goal so return status of still searching.
            return State.Searching;
        }

        private AStarData GetData(IGraphNode node)
        {
            if (_dataDict.TryGetValue(node, out var data))
            {
                return data;
            }

            data = new AStarData();
            _dataDict.Add(node, data);

            return data;
        }

        /// <summary>
        /// Gets the path of the last solution of the AStar algorithm.
        /// Will return a partial path if the algorithm has not finished yet.
        /// </summary>
        /// <returns>Returns empty if the algorithm has never been run.</returns>
        public IGraphNode[] GetPath()
        {
            if (CurrentNode == null)
            {
                return System.Array.Empty<IGraphNode>();
            }

            var next = CurrentNode;
            var path = new List<IGraphNode>();
            while (next != null)
            {
                path.Add(next);

                var nextData = GetData(next);

                next = nextData.Parent;
            }

            path.Reverse();
            return path.ToArray();
        }
    }


}

