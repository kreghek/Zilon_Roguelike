using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Math;
using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Tactics.Behaviour
{
    public class MoveToPointCommand: ICommand
    {
        private MapNode _targetNode;
        private CombatMap _map;
        private List<MapNode> _path;
        
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

        public MoveToPointCommand(IActor actor, MapNode targetNode, CombatMap map)
        {
            Actor = actor;
            _targetNode = targetNode;
            _map = map;

            _path = new List<MapNode>();

            CreatePath();
        }

        private void CreatePath()
        {
            var startNode = Actor.Node;
            var finishNode = _targetNode;
            
            _path.Clear();

            var directions = new[] {
               new Vector3(+1, -1, 0), new Vector3(+1, 0, -1), new Vector3(0, +1, -1),
               new Vector3(-1, +1, 0), new Vector3(-1, 0, +1), new Vector3(0, -1, +1)
            };

            var counter = 100;
            while (startNode != finishNode && counter > 0)
            {
                var nearbyNodes = _map.GetNeiberhoods(startNode);

                var finishCubeCoords = finishNode.GetCubeCoords();
                var nearbyDistances = nearbyNodes.Select(n => {
                    var cubeCoords = n.GetCubeCoords();

                    var b = cubeCoords;
                    var a = finishCubeCoords;

                    var distance1 = System.Math.Max(System.Math.Abs(a.X - b.X), System.Math.Abs(a.Y - b.Y));
                    var distance = System.Math.Max(distance1, System.Math.Abs(a.Z - b.Z));

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