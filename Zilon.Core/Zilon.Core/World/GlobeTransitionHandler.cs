﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.Tactics;

namespace Zilon.Core.World
{
    public sealed class GlobeTransitionHandler : IGlobeTransitionHandler, IDisposable
    {
        private const int TRANSITION_PER_GLOBE_ITERATION = 10;
        private readonly IGlobeExpander _globeExpander;

        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly ITransitionPool _transitionPool;

        public GlobeTransitionHandler(IGlobeExpander globeExpander, ITransitionPool transitionPool)
        {
            _globeExpander = globeExpander ?? throw new ArgumentNullException(nameof(globeExpander));
            _transitionPool = transitionPool ?? throw new ArgumentNullException(nameof(transitionPool));

            //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
            _semaphoreSlim = new SemaphoreSlim(1, 1);
        }

        private static bool FilterNodeToTransition(IGraphNode textNode, ISector nextSector)
        {
            var nodeIsBusyWithMonster = nextSector.ActorManager.Items.Any(x => x.Node == textNode);
            if (nodeIsBusyWithMonster)
            {
                return false;
            }

            var nodeIsBusyWithStaticObject = nextSector.StaticObjectManager.Items.Any(x => x.Node == textNode);
            if (nodeIsBusyWithStaticObject)
            {
                return false;
            }

            return true;
        }

        private async Task ProcessInnerAsync(IGlobe globe, ISector sector, IActor actor, SectorTransition transition)
        {
            var sectorNode = transition.SectorNode;

            //TODO Разобраться с этим кодом.
            // https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
            //Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released 
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (sectorNode.State != SectorNodeState.SectorMaterialized)
                {
                    await _globeExpander.ExpandAsync(sectorNode).ConfigureAwait(false);
                    globe.AddSectorNode(sectorNode);
                }

                // It was used as fallback later.
                var oldActorNode = actor.Node;

                var removingSuccessfull = true;
                try
                {
                    sector.ActorManager.Remove(actor);
                    removingSuccessfull = true;
                }
                catch (InvalidOperationException)
                {
                    Debug.Fail("Actor was removed successfully after transition.");
                    //throw;
                }

                if (removingSuccessfull)
                {
                    var nextSector = sectorNode.Sector;

                    if (nextSector is null)
                    {
                        throw new InvalidOperationException();
                    }

                    // Push in transition pool only if remove from previous sector is success.
                    // Otherwise 2 actors of same person will live in 2 different places.
                    var transitionItem =
                        new TransitionPoolItem(actor.Person, actor.TaskSource, nextSector, sector, oldActorNode);
                    _transitionPool.Push(transitionItem);
                }
                else
                {
                    throw new InvalidCastException("Actor was not correctly removed from previous sector.");
                }
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }
        }

        private static void TryToTransitPersonToTargetSector(TransitionPoolItem transitionItem)
        {
            var nextSector = transitionItem.NextSector;

            var transitionKeyPairsInNextSector = nextSector.Map.Transitions
                .Where(x => x.Value.SectorNode.Sector == transitionItem.OldSector);

            IGraphNode? nodeForTransition;
            if (transitionKeyPairsInNextSector.Any())
            {
                var transitionKeyPairInNextSector = transitionKeyPairsInNextSector.First();

                nodeForTransition = transitionKeyPairInNextSector.Key;
            }
            else
            {
                Debug.Fail("In target sector must be transition to source sector.");
                nodeForTransition = nextSector.Map.Nodes.FirstOrDefault();

                if (nodeForTransition is null)
                {
                    nodeForTransition = transitionItem.OldNode;
                }
            }

            var availableNextNodesToTransition = nextSector.Map.GetNext(nodeForTransition);

            var allPotentialNodesToTransition = new[] { nodeForTransition }.Concat(availableNextNodesToTransition);
            var allAvailableNodesToTransition = allPotentialNodesToTransition
                .Where(x => FilterNodeToTransition(x, nextSector)).ToArray();

            var availableNodeToTransition = allAvailableNodesToTransition.FirstOrDefault();
            if (availableNodeToTransition is null)
            {
                // I dont know what I can do.
                // I think it was solved when a some transition pool was developed.
                // Now just return person into old sector in old transition node.
                nextSector = transitionItem.OldSector;
                availableNodeToTransition = transitionItem.OldNode;
            }

            var actorInNewSector =
                new Actor(transitionItem.Person, transitionItem.TaskSource, availableNodeToTransition);
            nextSector.ActorManager.Add(actorInNewSector);
        }

        public void Dispose()
        {
            _semaphoreSlim.Dispose();
        }

        public Task InitActorTransitionAsync(IGlobe globe, ISector sector, IActor actor, SectorTransition transition)
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

        public void UpdateTransitions()
        {
            // The counter is restriction of transition per globe iteration.
            var counter = TRANSITION_PER_GLOBE_ITERATION;

            // Transit persons from pool to target sector levels while the pool is not empty or transition limit reached.
            do
            {
                var transitionItem = _transitionPool.Pop();

                if (transitionItem is null)
                {
                    return;
                }

                TryToTransitPersonToTargetSector(transitionItem);

                counter--;
            } while (counter > 0);
        }
    }
}