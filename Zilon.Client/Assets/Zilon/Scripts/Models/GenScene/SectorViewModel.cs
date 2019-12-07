using System;
using System.Linq;

using UnityEngine;

using Zenject;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

public class SectorViewModel : MonoBehaviour
{
    [Inject] private readonly IGlobeGenerator _globeGenerator;
    [Inject(Id = "monster")] private readonly IActorTaskSource _actorTaskSource;

    public SectorMapViewModel MapViewModel;
    public ActorsViewModel ActorsViewModel;

    private Globe _globe;
    private ISector _sector;
    private IActor _followedActor;

    private float _counter;

    private async void Start()
    {
        var globeGenerationResult = await _globeGenerator.CreateGlobeAsync();

        _globe = globeGenerationResult.Globe;

        var sectorInfo = _globe.SectorInfos.First();
        _sector = sectorInfo.Sector;
        MapViewModel.Init(_sector.Map);

        ActorsViewModel.Init(_sector.ActorManager);

        _followedActor = _sector.ActorManager.Items.First();
    }

    // Update is called once per frame
    void Update()
    {
        const float TURN_DURATION = 1f;

        _counter += Time.deltaTime;
        if (_counter < TURN_DURATION)
        {
            return;
        }

        _counter -= TURN_DURATION;

        foreach (var sectorInfo in _globe.SectorInfos)
        {
            var actorManager = sectorInfo.Sector.ActorManager;

            var snapshot = new SectorSnapshot(sectorInfo.Sector);

            NextTurn(actorManager, _actorTaskSource, snapshot);

            sectorInfo.Sector.Update();
        };
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
