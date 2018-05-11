using System.Linq;
using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Core.Services;
using Zilon.Core.Services.CombatEvents;
using Zilon.Core.Services.CombatMap;
using Zilon.Core.Tactics.Events;

class CombatWorldVM : MonoBehaviour
{

    public CombatMapVM Map;
    public SchemeLocator SchemeLocator;
    public Text Text;

    [Inject]
    private ICommandManager CommandManager;
    [Inject]
    private ICombatService CombatService;
    [Inject]
    private ICombatManager CombatManager;
    [Inject]
    private IEventManager EventManager;
    [Inject]
    private IMapGenerator mapGenerator;

    private void FixedUpdate()
    {
        ExecuteCommands();
        UpdateEvents();
    }

    private void UpdateEvents()
    {
        EventManager.Update();
    }

    private void ExecuteCommands()
    {
        var command = CommandManager.Pop();
        if (command == null)
            return;

        Debug.Log($"Executing {command}");

        command.Execute();
    }

    private void Awake()
    {
        var initData = CombatHelper.GetData(mapGenerator);
        var combat = CombatService.CreateCombat(initData);
        CombatManager.CurrentCombat = combat;
        EventManager.OnEventProcessed += EventManager_OnEventProcessed;

        Map.InitCombat();
    }

    private void EventManager_OnEventProcessed(object sender, CombatEventArgs e)
    {
        Debug.Log(e.CommandEvent);

        switch (e.CommandEvent.Id)
        {
            case "squad-moved":
                var combat = CombatManager.CurrentCombat;
                var squadMovedEvent = e.CommandEvent as SquadMovedEvent;
                var actorSquad = combat.Squads.SingleOrDefault(x => x.Id == squadMovedEvent.SquadId);
                var targetNode = combat.Map.Nodes.SingleOrDefault(x => x.Id == squadMovedEvent.FinishNodeId);

                Map.MoveSquad(actorSquad, targetNode);

                //TODO Добавить обработку завершения событий.

                break;

            case "event-group":
                var group = e.CommandEvent as EventGroup;

                EventManager.EventsToQueue(group.Events);

                //TODO Добавить обработка завершения событий.
                break;
        }
    }
}
