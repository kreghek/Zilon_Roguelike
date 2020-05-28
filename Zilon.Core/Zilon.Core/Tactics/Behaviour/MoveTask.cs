using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class MoveTask : ActorTaskBase
    {
        private readonly ISectorMap _map;
        private readonly int _cost;
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

                IsComplete = true;
                return;
            }

            var nextNode = _path[0];

            if (_map.IsPositionAvailableFor(nextNode, Actor))
            {
                ReleaseNodes(Actor);
                Actor.MoveToNode(nextNode);
                HoldNodes(nextNode, Actor);

                _path.RemoveAt(0);

                if (!_path.Any())
                {
                    IsComplete = true;
                }
            }
            else
            {
                IsComplete = true;
            }
        }

        private void HoldNodes(IGraphNode nextNode, IActor actor)
        {
            var actorNodes = GetActorNodes(actor.Person, nextNode);

            foreach (var node in actorNodes)
            {
                _map.HoldNode(node, actor);
            }
        }

        private void ReleaseNodes(IActor actor)
        {
            var actorNodes = GetActorNodes(actor.Person, actor.Node);

            foreach (var node in actorNodes)
            {
                _map.ReleaseNode(node, actor);
            }
        }

        private IEnumerable<IGraphNode> GetActorNodes(IPerson person, IGraphNode baseNode)
        {
            yield return baseNode;

            if (person.PhysicalSize == PhysicalSize.Size7)
            {
                var neighbors = _map.GetNext(baseNode);
                foreach (var neighbor in neighbors)
                {
                    yield return neighbor;
                }
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

        public override int Cost => _cost;

        public MoveTask(IActor actor, IActorTaskContext context, IGraphNode targetNode, ISectorMap map) : this(actor, context, targetNode, map, 1000)
        { 
        }

        public MoveTask(IActor actor, IActorTaskContext context, IGraphNode targetNode, ISectorMap map, int cost) : base(actor, context)
        {
            TargetNode = targetNode ?? throw new ArgumentNullException(nameof(targetNode));
            _map = map ?? throw new ArgumentNullException(nameof(map));
            _cost = cost;
            if (actor.Node == targetNode)
            {
                // Это может произойти, если источник команд выбрал целевую точку ту же, что и сам актёр
                // в результате рандома.
                IsComplete = true;

                _path = new List<IGraphNode>(0);
            }
            else
            {
                _path = new List<IGraphNode>();

                CreatePath();

                if (!_path.Any())
                {
                    IsComplete = true;
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