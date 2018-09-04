using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public sealed class GameManager : IGameManager
    {
        private readonly List<IActor> _actors;
        private readonly List<IActor> _processedActors;
        private readonly ISector _sector;
        private readonly IActorManager _actorManager;

        public GameManager(ISector sector, IActorManager actorManager)
        {
            _sector = sector;
            _actorManager = actorManager;

            _actors = new List<IActor>();
            RefillActorList();
        }

        public IActorTaskSource[] ActorTaskSources { get; set; }

        public async Task RequestNextActorTaskAsync()
        {
            var actor = _actors.FirstOrDefault();

            if (actor != null)
            {
                await ProcessActorAsync(actor);
            }
            else
            {
                _sector.Update();

                RefillActorList();

                actor = _actors.FirstOrDefault();
                await ProcessActorAsync(actor);
            }
        }

        private async Task ProcessActorAsync(IActor actor)
        {
            _actors.Remove(actor);

            if (ActorTaskSources == null)
            {
                return;
            }

            foreach (var taskSource in ActorTaskSources)
            {
                var actorTasks = await taskSource.GetActorTasksAsync(actor);

                foreach (var actorTask in actorTasks)
                {
                    actorTask.Execute();
                }
            }
        }

        private void RefillActorList()
        {
            var availableActors = _actorManager.Actors.Where(x => !x.State.IsDead).ToArray();

            // отсортировать по инициативе
            _actors.AddRange(availableActors); 
        }
    }
}
