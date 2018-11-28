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

        public GameLoop(ISector sector, IActorManager actorManager)
        {
            _sector = sector;
            _actorManager = actorManager;
        }

        public IActorTaskSource[] ActorTaskSources { get; set; }

        public void Update()
        {
            if (ActorTaskSources == null)
            {
                throw new InvalidOperationException("Не заданы источники команд");
            }

            var actorsQueue = CalcActorList();

            //TODO Учитывать, что могут быть другие персонажи актёра (псы, участники взвода/группы)
            var firstIsHumanPlayer = actorsQueue.FirstOrDefault()?.Owner is HumanPlayer;
            if (!firstIsHumanPlayer && actorsQueue.Any(x => x.Owner is HumanPlayer))
            {
                throw new InvalidOperationException("Первым должен быть персонаж, которым управляет актёр");
            }

            foreach (var actor in actorsQueue)
            {
                if (actor.Person.Survival.IsDead)
                {
                    continue;
                }

                ProcessActor(actor);
            }

            _sector.Update();
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

        private IActor[] CalcActorList()
        {
            // Персонаж, которым в данный момент управляет актёр, должен обрабатываться первым.
            var sortedActors = _actorManager.Items.Where(x => !x.Person.Survival.IsDead)
                .OrderByDescending(x => x.Owner is HumanPlayer)
                .ThenBy(x=>x.Person.Id)
                .ToArray();

            // отсортировать по инициативе
            return sortedActors;
        }
    }
}
