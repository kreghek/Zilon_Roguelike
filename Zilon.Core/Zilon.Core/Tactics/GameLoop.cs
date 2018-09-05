using System;
using System.Linq;

using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public sealed class GameLoop : IGameLoop
    {
        private readonly ISector _sector;
        private readonly IActorManager _actorManager;

        private IActor[] _actorsQueue;

        public GameLoop(ISector sector, IActorManager actorManager)
        {
            _sector = sector;
            _actorManager = actorManager;

            _actorsQueue = new IActor[0];
            RefillActorList();
        }

        public IActorTaskSource[] ActorTaskSources { get; set; }

        public void Update()
        {
            if (ActorTaskSources == null)
            {
                throw new InvalidOperationException("Не заданы источники команд");
            }

            //TODO Учитывать, что могут быть другие персонажи актёра (псы, участники взвода/группы)
            var firstIsHumanPlayer = _actorsQueue.FirstOrDefault().Owner is HumanPlayer;
            if (!firstIsHumanPlayer)
            {
                throw new InvalidOperationException("Первым должен быть персонаж, которым управляет актёр");
            }

            foreach (var actor in _actorsQueue)
            {
                if (actor.State.IsDead)
                {
                    continue;
                }

                ProcessActor(actor);
            }

            _sector.Update();

            RefillActorList();
        }

        private void ProcessActor(IActor actor)
        {
            foreach (var taskSource in ActorTaskSources)
            {
                var actorTasks = taskSource.GetActorTasks(actor);

                foreach (var actorTask in actorTasks)
                {
                    actorTask.Execute();
                }
            }
        }

        private void RefillActorList()
        {
            // Персонаж, которым в данный момент управляет актёр, должен обрабатываться первым.
            var sortedActors = _actorManager.Actors.Where(x => !x.State.IsDead)
                .OrderByDescending(x=>x.Owner is HumanPlayer)
                .ToArray();

            // отсортировать по инициативе
            _actorsQueue = sortedActors; 
        }
    }
}
