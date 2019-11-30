using System.Collections.Generic;
using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics.Spatial.PathFinding
{
    /// <summary>
    /// Interface to setup and run the AStar algorithm.
    /// </summary>
    public sealed class AStar
    {
        /// <summary>
        /// The open list.
        /// </summary>
        private readonly SortedList<int, IGraphNode> _openList;

        /// <summary>
        /// The closed list.
        /// </summary>
        private readonly SortedList<int, IGraphNode> _closedList;


        private readonly IMap _map;
        private readonly IPathFindingContext _context;
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
        /// <param name="map">Карта, на которой выполнять поиск.</param>
        /// <param name="context"> Контекст выполнения поиска (способности персонажа, служебная информация). </param>
        /// <param name="start">The starting node for the AStar algorithm.</param>
        /// <param name="goal">The goal node for the AStar algorithm.</param>
        public AStar(IMap map, IPathFindingContext context, IGraphNode start, IGraphNode goal)
        {
            if (start == null)
            {
                throw new System.ArgumentNullException(nameof(start));
            }

            if (goal == null)
            {
                throw new System.ArgumentNullException(nameof(goal));
            }

            var duplicateComparer = new DuplicateComparer();
            _openList = new SortedList<int, IGraphNode>(duplicateComparer);
            _closedList = new SortedList<int, IGraphNode>(duplicateComparer);
            _dataDict = new Dictionary<IGraphNode, AStarData>();

            _map = map ?? throw new System.ArgumentNullException(nameof(map));
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
            _openList.Add(CurrentNode, currentData);

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
                var s = Step();
                if (s != State.Searching)
                {
                    return s;
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
                if (_closedList.ContainsValue(CurrentNode))
                {
                    continue;
                }

                // An unsearched node has been found, search it.
                break;
            }

            // Remove from the open list and place on the closed list 
            // since this node is now being searched.

            var currentData = GetData(CurrentNode);

            _openList.Remove(AStarData.TotalCost);

            _closedList.Add(CurrentNode, currentData);

            // Found the goal, stop searching.
            if (CurrentNode == _goal)
            {
                return State.GoalFound;
            }

            // Node was not the goal so add all children nodes to the open list.
            // Each child needs to have its movement cost set and estimated cost.

            var neighbors = GetAvailableNeighbors(CurrentNode, _map);

            foreach (var child in neighbors)
            {
                // If the child has already been searched (closed list) or is on
                // the open list to be searched then do not modify its movement cost
                // or estimated cost since they have already been set previously.
                if (_openList.ContainsValue(child) || _closedList.ContainsValue(child))
                {
                    continue;
                }

                var childData = GetData(child);
                currentData = GetData(CurrentNode);


                childData.Parent = CurrentNode;
                childData.MovementCost = currentData.MovementCost + 1;

                _openList.Add(child, childData);
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
        /// Возвращает доступные соседние узлы карты с учётом обхода соседей по часовой стрелке.
        /// </summary>
        /// <param name="current"> Текущий узел. </param>
        /// <param name="map"> Карта, на которой проводится проверка. </param>
        /// <returns> Возвращает список соседних узлов, соединённых ребрами с текущим. </returns>
        private IGraphNode[] GetAvailableNeighbors(IGraphNode current, IMap map)
        {
            var hexCurrent = (HexNode)current;
            var neighbors = map.GetNext(hexCurrent);

            var actualNeighbors = new List<IGraphNode>();
            foreach (var testedNeighbor in neighbors)
            {
                if (_context.TargetNode == null)
                {
                    if (!map.IsPositionAvailableFor(testedNeighbor, _context.Actor))
                    {
                        continue;
                    }
                }
                else
                {
                    var isNotAvailable = !IsAvailable(map, testedNeighbor);
                    if (isNotAvailable)
                    {
                        continue;
                    }
                }

                actualNeighbors.Add(testedNeighbor);
            }

            return actualNeighbors.ToArray();
        }

        private bool IsAvailable(IMap map, IGraphNode testedNeighbor)
        {
            if (_context.TargetNode == testedNeighbor)
            {
                return true;
            }

            if (map.IsPositionAvailableFor(testedNeighbor, _context.Actor))
            {
                return true;
            }

            return false;
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
                return new IGraphNode[0];
            }

            var next = CurrentNode;
            var path = new List<IGraphNode>();
            while (next != null)
            {
                if (_map.IsPositionAvailableFor(next, _context.Actor))
                {
                    path.Add(next);
                }

                var nextData = GetData(next);

                next = nextData.Parent;
            }
            path.Reverse();
            return path.ToArray();
        }
    }


}

