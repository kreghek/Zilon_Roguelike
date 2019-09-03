using System;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public sealed class GameLoop : IGameLoop
    {
        private readonly ISectorManager _sectorManager;
        private readonly IActorManager _actorManager;

        public event EventHandler Updated;

        public GameLoop(ISectorManager sectorManager, IActorManager actorManager)
        {
            _sectorManager = sectorManager;
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

            var firstIsHumanPlayer = actorsQueue.FirstOrDefault()?.Owner is HumanPlayer;
            if (!firstIsHumanPlayer && actorsQueue.Any(x => x.Owner is HumanPlayer))
            {
                throw new InvalidOperationException("Первым должен быть персонаж, которым управляет актёр");
            }

            foreach (var actor in actorsQueue)
            {
                if (actor.Person.CheckIsDead())
                {
                    continue;
                }

                ProcessActor(actor);
            }

            _sectorManager.CurrentSector.Update();

            Updated?.Invoke(this, new EventArgs());
        }

        private void ProcessActor(IActor actor)
        {
            foreach (var taskSource in ActorTaskSources)
            {
                var actorTasks = taskSource.GetActorTasks(actor);

                foreach (var actorTask in actorTasks)
                {
                    try
                    {
                        actorTask.Execute();
                    }
                    catch (Exception exception)
                    {
                        throw new ActorTaskExecutionException($"Ошибка при работе источника команд {taskSource.GetType().FullName}",
                            taskSource,
                            exception);
                    }
                }
            }
        }

        private IActor[] CalcActorList()
        {
            // Персонаж, которым в данный момент управляет актёр, должен обрабатываться первым.
            var sortedActors = _actorManager.Items.Where(x => !x.Person.CheckIsDead())
                .OrderByDescending(x => x.Owner is HumanPlayer)
                .ThenBy(x=>x.Person.Id)
                .ToArray();

            // отсортировать по инициативе
            return sortedActors;
        }
    }
}
