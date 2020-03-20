using System;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

public class GlobeIterator : MonoBehaviour
{
    [Inject] private readonly IGlobeManager _globeManager;

    private float _counter;

    public SectorViewModel SectorViewModel;

    [Inject(Id = "monster")] private readonly IActorTaskSource _actorTaskSource;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members",
        Justification = "Неявно вызывается Unity.")]
    private void FixedUpdate()
    {
        var canUpdateCurrentGlobe = _globeManager.IsGlobeInitialized && SectorViewModel.IsInitialized;
        if (!canUpdateCurrentGlobe)
        {
            return;
        }

        const float TURN_DURATION = 1f;

        _counter += Time.fixedDeltaTime;
        if (_counter < TURN_DURATION)
        {
            return;
        }

        _counter -= TURN_DURATION;

        var globe = _globeManager.Globe;
        foreach (var sectorInfo in globe.SectorInfos)
        {
            var actorManager = sectorInfo.Sector.ActorManager;

            var snapshot = new SectorSnapshot(sectorInfo.Sector);

            NextTurn(actorManager, _actorTaskSource, snapshot);

            sectorInfo.Sector.Update();
        };

        globe.Iteration++;
    }

    private static void NextTurn(IActorManager actors, IActorTaskSource taskSource, SectorSnapshot snapshot)
    {
        foreach (var actor in actors.Items)
        {
            if (actor.Person.CheckIsDead())
            {
                continue;
            }

            ProcessActor(actor, taskSource, snapshot);
        }
    }

    private static void ProcessActor(IActor actor, IActorTaskSource taskSource, SectorSnapshot snapshot)
    {
        var actorTasks = taskSource.GetActorTasks(actor, snapshot);

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
