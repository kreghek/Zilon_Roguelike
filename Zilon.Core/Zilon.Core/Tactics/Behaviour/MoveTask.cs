using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class MoveTask: IActorTask
    {
        private readonly HexNode _targetNode;
        private readonly IMap _map;
        private readonly List<HexNode> _path;
        
        public IActor Actor { get; }
        public void Execute()
        {
            if (!_path.Any())
            {
                IsComplete = true;
                return;
            }

            var nextNode = _path[0];
            Actor.MoveToNode(nextNode);
            _path.RemoveAt(0);

            if (!_path.Any())
            {
                IsComplete = true;
                return;
            }
        }

        public bool IsComplete { get; set; }

        public MoveTask(IActor actor, HexNode targetNode, IMap map)
        {
            Actor = actor;
            _targetNode = targetNode;
            _map = map;

            _path = new List<HexNode>();

            CreatePath();
        }

        private void CreatePath()
        {
            var startNode = Actor.Node;
            var finishNode = _targetNode;
            
            _path.Clear();

            var directions = new[] {
               new CubeCoords(+1, -1, 0), new CubeCoords(+1, 0, -1), new CubeCoords(0, +1, -1),
               new CubeCoords(-1, +1, 0), new CubeCoords(-1, 0, +1), new CubeCoords(0, -1, +1)
            };

            var counter = 100;
            while (startNode != finishNode && counter > 0)
            {
                var nearbyNodes = HexNodeHelper.GetNeighbors(startNode, _map.Nodes);

                var finishCubeCoords = finishNode.CubeCoords;
                var nearbyDistances = nearbyNodes.Select(n => {
                    var cubeCoords = n.CubeCoords;

                    var distance = cubeCoords.DistanceTo(finishCubeCoords);

                    return new {
                        Distance = distance,
                        Node = n
                    };
                }).OrderBy(x=>x.Distance);

                var best = nearbyDistances.First().Node;

                _path.Add(best);
                startNode = best;

                counter--;
            }
        }
    }
}