using System;
using System.Linq;
using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Models.Commands;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Core.Commands;
using Zilon.Core.Services;
using Zilon.Core.Services.CombatEvents;
using Zilon.Core.Services.CombatMap;
using Zilon.Core.Tactics.Events;

class CombatWorldVM : MonoBehaviour
{

    private float turnCounter;

    public CombatMapVM Map;
    public SchemeLocator SchemeLocator;
    public Text Text;

    [Inject]
    private ICommandManager _commandManager;
    [Inject]
    private ICombatService _combatService;
    [Inject]
    private ICombatManager _combatManager;
    [Inject]
    private IEventManager _eventManager;
    [Inject]
    private IMapGenerator _mapGenerator;

    [Inject(Id = "squad-command-factory")]
    private ICommandFactory _commandFactory;

    private void FixedUpdate()
    {
        ExecuteCommands();
        UpdateEvents();
        UpdateTurnCounter();
    }

    private void UpdateTurnCounter()
    {
        turnCounter += Time.deltaTime;
        if (turnCounter < 10)
        {
            return;
        }

        turnCounter = 0;

        var endTurnCommand = _commandFactory.CreateCommand<EndTurnCommand>();
        _commandManager.Push(endTurnCommand);
    }

    private void UpdateEvents()
    {
        _eventManager.Update();
    }

    private void ExecuteCommands()
    {
        var command = _commandManager?.Pop();
        if (command == null)
            return;

        Debug.Log($"Executing {command}");

        command.Execute();
    }

    private void Awake()
    {
        var initData = CombatHelper.GetData(_mapGenerator);
        var combat = _combatService.CreateCombat(initData);
        _combatManager.CurrentCombat = combat;
        _eventManager.OnEventProcessed += EventManager_OnEventProcessed;

        Map.InitCombat();
    }

    private void EventManager_OnEventProcessed(object sender, CombatEventArgs e)
    {
        Debug.Log(e.CommandEvent);

        switch (e.CommandEvent.Id)
        {
            case "squad-moved":
                var combat = _combatManager.CurrentCombat;
                var squadMovedEvent = e.CommandEvent as SquadMovedEvent;
                if (squadMovedEvent == null)
                {
                    throw new ArgumentNullException(nameof(squadMovedEvent));
                }

                var actorSquad = combat.Squads.SingleOrDefault(x => x.Id == squadMovedEvent.SquadId);
                var targetNode = combat.Map.Nodes.SingleOrDefault(x => x.Id == squadMovedEvent.FinishNodeId);

                Map.MoveSquad(actorSquad, targetNode);

                //TODO Добавить обработку завершения событий.

                break;

            case "event-group":
                var eventGroup = e.CommandEvent as EventGroup;
                if (eventGroup == null)
                {
                    throw new ArgumentNullException(nameof(eventGroup));
                }

                _eventManager.EventsToQueue(eventGroup.Events);

                //TODO Добавить обработка завершения событий.
                break;

            default:
                throw new InvalidOperationException("Неизвесный тип события");
        }
    }
}
