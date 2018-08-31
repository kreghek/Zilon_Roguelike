using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public sealed class GameManager
    {
        private readonly List<IActor> _actors = new List<IActor>();
        private readonly IActorTaskSource[] _actorTaskSources;
        private readonly ISector sector;
        private IActorManager _actorManager;

        public async void RequestNextActorTaskAsync()
        {
            var actor = _actors.FirstOrDefault();

            if (actor != null)
            {
                await ProcessActorAsync(actor);
            }
            else
            {
                sector.Update();
                _actors.AddRange(_actorManager.Actors); // отсортировать по инициативе

                actor = _actors.FirstOrDefault();
                await ProcessActorAsync(actor);
            }
        }

        private async Task ProcessActorAsync(IActor actor)
        {
            _actors.Remove(actor);

            foreach (var taskSource in _actorTaskSources)
            {
                Task task = taskSource.GetActorTasks(actor);
                await task;
            }
        }
    }
}
