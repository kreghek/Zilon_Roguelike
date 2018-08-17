using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tactics.Spatial.PathFinding;

namespace Zilon.Core.Tactics.Behaviour
{
    public class MoveTask: ActorTaskBase
    {
        private readonly IMap _map;
        private readonly List<IMapNode> _path;

        public IMapNode TargetNode { get; }

        public override void Execute()
        {
            if (!_path.Any())
            {
                IsComplete = true;
                return;
            }

            //TODO Добавить тест, исправить баг.
            // Этой проверки быть не должно, потому что _path и так содержит
            // цепочку узлов до целевого. И должно выполняться верхнее условие.
            if (Actor.Node == TargetNode)
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
            }
        }

        public MoveTask(IActor actor, IMapNode targetNode, IMap map): base(actor)
        {
            TargetNode = targetNode;
            _map = map;

            _path = new List<IMapNode>();

            CreatePath();
        }

        private void CreatePath()
        {
            var startNode = Actor.Node;
            var finishNode = TargetNode;
            
            _path.Clear();

            var astar = new AStar(_map, startNode, finishNode);
            var resultState = astar.Run();
            if (resultState == State.GoalFound)
            {
                var foundPath = astar.GetPath().Skip(1).ToArray();
                foreach (var pathNode in foundPath)
                {
                    _path.Add((HexNode)pathNode);
                }
            }
        }

        public override string ToString()
        {
            return $"{Actor} ({TargetNode})";
        }
    }
}