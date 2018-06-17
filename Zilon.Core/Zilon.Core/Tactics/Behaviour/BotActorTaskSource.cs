using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public class BotActorTaskSource : IActorTaskSource
    {
        private Random _rnd = new Random();

        private readonly Dictionary<IActor, IActorTask> _taskDict;
        private readonly IPlayer _player;

        public BotActorTaskSource(IPlayer player)
        {
            _taskDict = new Dictionary<IActor, IActorTask>();
            _player = player;
        }

        public IActorTask[] GetActorTasks(IMap map, IActor[] actors)
        {
            foreach (var actor in actors)
            {
                if (actor.Person.Player != _player)
                {
                    continue;
                }

                if (actor.IsDead)
                {
                    continue;
                }

                if (!_taskDict.TryGetValue(actor, out IActorTask task))
                {
                    var nodeRoll = _rnd.Next(map.Nodes.Count());
                    var targetNode = map.Nodes[nodeRoll];
                    var moveTask = new MoveTask(actor, targetNode, map);
                    _taskDict.Add(actor, moveTask);
                }
            }

            var taskedActors = _taskDict.Keys.ToArray();
            foreach (var taskedActor in taskedActors)
            {
                var task = _taskDict[taskedActor];
                if (task.IsComplete)
                {
                    _taskDict.Remove(taskedActor);
                }
            }

            return _taskDict.Values.ToArray();
        }
    }
}
