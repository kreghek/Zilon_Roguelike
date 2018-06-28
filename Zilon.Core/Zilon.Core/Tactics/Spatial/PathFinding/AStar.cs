using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Tactics.Spatial.PathFinding
{
    /// <summary>
    /// Interface to setup and run the AStar algorithm.
    /// </summary>
    public class AStar
    {
        /// <summary>
        /// The open list.
        /// </summary>
        private readonly SortedList<int, IMapNode> _openList;

        /// <summary>
        /// The closed list.
        /// </summary>
        private readonly SortedList<int, IMapNode> _closedList;


        private readonly IMap _map;

        private readonly Dictionary<IMapNode, AStarData> _dataDict;

        /// <summary>
        /// The current node.
        /// </summary>
        private IMapNode _current;

        /// <summary>
        /// The goal node.
        /// </summary>
        private IMapNode _goal;

        /// <summary>
        /// Gets the current amount of steps that the algorithm has performed.
        /// </summary>
        public int Steps { get; private set; }

        /// <summary>
        /// Gets the current state of the open list.
        /// </summary>
        public IEnumerable<IMapNode> OpenList => _openList.Values;

        /// <summary>
        /// Gets the current state of the closed list.
        /// </summary>
        public IEnumerable<IMapNode> ClosedList => _closedList.Values;

        /// <summary>
        /// Gets the current node that the AStar algorithm is at.
        /// </summary>
        public IMapNode CurrentNode => _current;

        /// <summary>
        /// Creates a new AStar algorithm instance with the provided start and goal nodes.
        /// </summary>
        /// <param name="map">Карта, на которой выполнять поиск.</param>
        /// <param name="start">The starting node for the AStar algorithm.</param>
        /// <param name="goal">The goal node for the AStar algorithm.</param>
        public AStar(IMap map, IMapNode start, IMapNode goal)
        {
            var duplicateComparer = new DuplicateComparer();
            _openList = new SortedList<int, IMapNode>(duplicateComparer);
            _closedList = new SortedList<int, IMapNode>(duplicateComparer);
            _dataDict = new Dictionary<IMapNode, AStarData>();

            _map = map;

            Reset(start, goal);
        }

        /// <summary>
        /// Resets the AStar algorithm with the newly specified start node and goal node.
        /// </summary>
        /// <param name="start">The starting node for the AStar algorithm.</param>
        /// <param name="goal">The goal node for the AStar algorithm.</param>
        public void Reset(IMapNode start, IMapNode goal)
        {
            _openList.Clear();
            _closedList.Clear();
            _dataDict.Clear();

            _current = start;
            _goal = goal;

            var currentData = GetData(_current);
            _openList.Add(_current, currentData);
            
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
                State s = Step();
                if (s != State.Searching)
                    return s;
            }
        }

        /// <summary>
        /// Moves the AStar algorithm forward one step.
        /// </summary>
        /// <returns>Returns the state the alorithm is in after the step, either Failed, GoalFound or still Searching.</returns>
        public State Step()
        {
            Steps++;
            while (true)
            {
                // There are no more nodes to search, return failure.
                if (_openList.IsEmpty())
                {
                    return State.Failed;
                }

                // Check the next best node in the graph by TotalCost.
                _current = _openList.Pop();

                // This node has already been searched, check the next one.
                if (_closedList.ContainsValue(_current))
                {
                    continue;
                }

                // An unsearched node has been found, search it.
                break;
            }

            // Remove from the open list and place on the closed list 
            // since this node is now being searched.

            var currentData = GetData(_current);

            _openList.Remove(currentData.TotalCost);
            
            _closedList.Add(_current, currentData);

            // Found the goal, stop searching.
            if (_current == _goal)
            {
                return State.GoalFound;
            }

            // Node was not the goal so add all children nodes to the open list.
            // Each child needs to have its movement cost set and estimated cost.

            var neighbors = GetAvailableNeighbors(_current, _map);

            foreach (var child in neighbors)
            {
                // If the child has already been searched (closed list) or is on
                // the open list to be searched then do not modify its movement cost
                // or estimated cost since they have already been set previously.
                if (OpenList.Contains(child) || ClosedList.Contains(child))
                {
                    continue;
                }

                var childData = GetData(child);
                currentData = GetData(_current);
                

                childData.Parent = _current;
                childData.MovementCost = currentData.MovementCost + 1;
                childData.EstimateCost = CalcEstimateCost(child);

                _openList.Add(child, childData);
            }

            // This step did not find the goal so return status of still searching.
            return State.Searching;
        }

        protected virtual int CalcEstimateCost(IMapNode node) {
            var hexGoal = (HexNode)_goal;
            var hexNode = (HexNode)node;
            return hexGoal.CubeCoords.DistanceTo(hexNode.CubeCoords);
        }

        private AStarData GetData(IMapNode node)
        {
            if (!_dataDict.TryGetValue(node, out var data))
            {
                data = new AStarData();
                _dataDict.Add(node, data);
            }

            return data;
        }

        /// <summary>
        /// Возвращает доступные соседние узлы карты с учётом обхода соседей по часовой стрелке.
        /// </summary>
        /// <param name="current"> Текущий узел. </param>
        /// <param name="map"> Карта, на которой проводится проверка. </param>
        /// <returns> Возвращает список соседних узлов, соединённых ребрами с текущим. </returns>
        private static IMapNode[] GetAvailableNeighbors(IMapNode current, IMap map)
        {
            var hexCurrent = (HexNode)current;
            var hexNodes = map.Nodes.Cast<HexNode>().ToArray();
            var neighbors = HexNodeHelper.GetNeighbors(hexCurrent, hexNodes);

            var currentEdges = from edge in map.Edges
                               where edge.Nodes.Contains(current)
                               select edge;
            var currentEdgeArray = currentEdges.ToArray();

            var actualNeighbors = new List<HexNode>();
            for (var i = 0; i < neighbors.Length; i++)
            {
                var testedNeighbor = neighbors[i];
                var edge = currentEdgeArray.SingleOrDefault(x => x.Nodes.Contains(testedNeighbor));
                if (edge != null)
                {
                    actualNeighbors.Add(testedNeighbor);
                }
            }

            return actualNeighbors.ToArray();
        }

        /// <summary>
        /// Gets the path of the last solution of the AStar algorithm.
        /// Will return a partial path if the algorithm has not finished yet.
        /// </summary>
        /// <returns>Returns null if the algorithm has never been run.</returns>
        public IMapNode[] GetPath()
        {
            if (_current == null)
            {
                return null;
            }

            var next = _current;
            var path = new List<IMapNode>();
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

