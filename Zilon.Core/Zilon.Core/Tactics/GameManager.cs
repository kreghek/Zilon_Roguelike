using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public sealed class GameManager : IGameManager
    {
        private readonly List<IActor> _actors = new List<IActor>();
        private readonly ISector _sector;
        private readonly IActorManager _actorManager;

        public GameManager(ISector sector, IActorManager actorManager)
        {
            _sector = sector;
            _actorManager = actorManager;
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
                _actors.AddRange(_actorManager.Actors); // отсортировать по инициативе

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
                var task = taskSource.GetActorTasks(actor);
                var actorTasks = await task;

                foreach (var actorTask in actorTasks)
                {
                    actorTask.Execute();
                }
            }
        }
    }
}
