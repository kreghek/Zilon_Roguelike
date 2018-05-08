using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Logic.Services;
using Zilon.Logic.Services.CombatEvents;

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
        var initData = CombatHelper.GetData();
        var combat = CombatService.CreateCombat(initData);
        CombatManager.CurrentCombat = combat;
        EventManager.OnEventProcessed += EventManager_OnEventProcessed;

        Map.InitCombat();
    }

    private void EventManager_OnEventProcessed(object sender, CombatEventArgs e)
    {
        Debug.Log(e.CommandEvent);
    }
}
