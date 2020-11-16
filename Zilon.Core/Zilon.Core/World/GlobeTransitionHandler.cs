using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public sealed class GlobeTransitionHandler : IGlobeTransitionHandler, IDisposable
    {
        private readonly IGlobeExpander _globeExpander;

        private readonly SemaphoreSlim _semaphoreSlim;

        public GlobeTransitionHandler(IGlobeExpander globeExpander)
        {
            _globeExpander = globeExpander ?? throw new ArgumentNullException(nameof(globeExpander));

            //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        public Task ProcessAsync(
            IGlobe globe,
            ISector sector,
            IActor actor,
            RoomTransition transition)
        {
            if (globe is null)
            {
                throw new ArgumentNullException(nameof(globe));
            }

            if (sector is null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            if (actor is null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (transition is null)
            {
                throw new ArgumentNullException(nameof(transition));
            }

            return ProcessInnerAsync(globe, sector, actor, transition);
        }

        private async Task ProcessInnerAsync(
            IGlobe globe,
            ISector sector,
            IActor actor,
            RoomTransition transition)
        {
            var sectorNode = transition.SectorNode;

            //TODO Разобраться с этим кодом.
            // https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
            //Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released 
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await _semaphoreSlim.WaitAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            try
            {
                if (sectorNode.State != SectorNodeState.SectorMaterialized)
                {
                    await _globeExpander.ExpandAsync(sectorNode).ConfigureAwait(false);
                    globe.AddSectorNode(sectorNode);
                }

                try
                {
                    sector.ActorManager.Remove(actor);
                }
                catch (InvalidOperationException exception)
                {
                    // Пока ничего не делаем
                    Console.WriteLine(exception);
                    Console.WriteLine(sector.GetHashCode());
                    Console.WriteLine(actor);
                }

                var nextSector = sectorNode.Sector;
                var nodeForTransition = nextSector.Map.Transitions.First(x => x.Value.SectorNode.Sector == sector).Key;
                var actorInNewSector = new Actor(actor.Person, actor.TaskSource, nodeForTransition);
                nextSector.ActorManager.Add(actorInNewSector);
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }
        }

        public void Dispose()
        {
            _semaphoreSlim.Dispose();
        }
    }
}