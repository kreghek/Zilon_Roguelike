using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Logic.Services;

class CombatWorldVM : MonoBehaviour
{

    public CombatMapVM Map;
    public SchemeLocator SchemeLocator;
    public Text Text;

    [Inject]
    public ICommandManager CommandManager;
    [Inject]
    public ICombatService CombatService;
    [Inject]
    private ICombatManager CombatManager;

    private void FixedUpdate()
    {
        ExecuteCommands();
    }

    private void ExecuteCommands()
    {
        var command = CommandManager.Pop();
        if (command == null)
            return;

        command.Execute();
    }

    private void Awake()
    {
        var initData = CombatHelper.GetData();
        var combat = CombatService.CreateCombat(initData);
        CombatManager.CurrentCombat = combat;

        Map.InitCombat();
    }
}
