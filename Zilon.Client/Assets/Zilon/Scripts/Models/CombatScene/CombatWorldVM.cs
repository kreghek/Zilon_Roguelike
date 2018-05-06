using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Models.Commands;
using Assets.Zilon.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;
using Zilon.Logic.Services;
using Zilon.Logic.Tactics;

class CombatWorldVM : MonoBehaviour
{

    public CombatMapVM Map;
    public SchemeLocator SchemeLocator;
    public Text Text;

    private readonly ICommandManager commandManager;
    private readonly CombatService combatService;

    private Combat combat;

    public CombatWorldVM(CombatService combatService, ICommandManager commandManager)
    {
        this.commandManager = commandManager;
        this.combatService = combatService;
    }

    private void FixedUpdate()
    {
        ExecuteCommands();
    }

    private void ExecuteCommands()
    {
        var command = commandManager.Pop();
        if (command == null)
            return;

        var combatCommand = command as ICommand<ICombatCommandContext>;
        if (combatCommand != null)
        {
            var combatCommandContext = new CombatCommandContext(combat);
            combatCommand.Execute(combatCommandContext);
        }
    }

    private void Awake()
    {
        var initData = CombatHelper.GetData();
        var combat = combatService.CreateCombat(initData);
        Map.InitCombat(combat);

        this.combat = combat;
    }
}
