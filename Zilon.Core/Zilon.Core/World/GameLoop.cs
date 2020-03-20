using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.World
{
    public sealed class GameLoop : IGameLoop
    {
        private readonly ITaskSourceProvider _taskSourceProvider;

        public event EventHandler Updated;

        public GameLoop(ITaskSourceProvider taskSourceProvider)
        {
            _taskSourceProvider = taskSourceProvider;
        }

        public Task UpdateAsync(Globe globe)
        {
            if (globe is null)
            {
                throw new ArgumentNullException(nameof(globe));
            }

            return Task.Run(() =>
            {
                foreach (var sectorInfo in globe.SectorInfos)
                {
                    var sectorActorManager = sectorInfo.Sector.ActorManager;
                    var actorsQueue = CalcActorList(sectorActorManager);

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

                        var sectorSnapshot = new SectorSnapshot(sectorInfo.Sector);
                        ProcessActor(actor, sectorSnapshot);
                    }

                    sectorInfo.Sector.Update();
                }

                Updated?.Invoke(this, new EventArgs());
            });
        }

        private void ProcessActor(IActor actor, SectorSnapshot sectorSnapshot)
        {
            var ActorTaskSources = _taskSourceProvider.GetTaskSources();
            foreach (var taskSource in ActorTaskSources)
            {
                var actorTasks = taskSource.GetActorTasks(actor, sectorSnapshot);

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

        private IActor[] CalcActorList(IActorManager sectorActorManager)
        {
            // Персонаж, которым в данный момент управляет актёр, должен обрабатываться первым.
            var sortedActors = sectorActorManager.Items.Where(x => !x.Person.CheckIsDead())
                .OrderByDescending(x => x.Owner is HumanPlayer)
                .ThenBy(x => x.Person.Id)
                .ToArray();

            // отсортировать по инициативе
            return sortedActors;
        }
    }
}
