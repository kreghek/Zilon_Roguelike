using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Graphs;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class MoveTask : ActorTaskBase
    {
        private readonly ISectorMap _map;
        private readonly List<IGraphNode> _path;

        public IGraphNode TargetNode { get; }

        public override void Execute()
        {
            if (IsComplete)
            {
                return;
            }

            if (!_path.Any())
            {
                if (TargetNode != Actor.Node)
                {
                    throw new TaskException("Актёр не достиг целевого узла при окончании пути.");
                }

                _isComplete = true;
                return;
            }

            var nextNode = _path[0];

            if (!_map.IsPositionAvailableFor(nextNode, Actor))
            {
                throw new InvalidOperationException($"Попытка переместиться в заблокированную ячейку {nextNode}.");
            }

            _map.ReleaseNode(Actor.Node, Actor);
            Actor.MoveToNode(nextNode);
            _map.HoldNode(nextNode, Actor);

            const int DISTANCE_OF_SIGN = 5;
            FowHelper.UpdateFowData(Actor, _map, nextNode, DISTANCE_OF_SIGN);

            _path.RemoveAt(0);

            if (!_path.Any())
            {
                _isComplete = true;
            }
        }

        public bool CanExecute()
        {
            if (!_path.Any())
            {
                return false;
            }

            var nextNode = _path[0];

            return _map.IsPositionAvailableFor(nextNode, Actor);
        }

        public MoveTask(IActor actor, IGraphNode targetNode, ISectorMap map) : base(actor)
        {
            TargetNode = targetNode ?? throw new ArgumentNullException(nameof(targetNode));
            _map = map ?? throw new ArgumentNullException(nameof(map));

            if (actor.Node == targetNode)
            {
                // Это может произойти, если источник команд выбрал целевую точку ту же, что и сам актёр
                // в результате рандома.
                _isComplete = true;

                _path = new List<IGraphNode>(0);
            }
            else
            {
                _path = new List<IGraphNode>();

                CreatePath();

                if (!_path.Any())
                {
                    _isComplete = true;
                }
            }
        }

        private void CreatePath()
        {
            var context = new ActorPathFindingContext(Actor, _map, TargetNode);

            var startNode = Actor.Node;
            var finishNode = TargetNode;

            _path.Clear();

            _map.FindPath(startNode, finishNode, context, _path);
        }

        public override string ToString()
        {
            return $"{Actor} ({TargetNode})";
        }
    }
}