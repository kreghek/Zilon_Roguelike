using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Models.Commands;
using Assets.Zilon.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zilon.Logic.Services;
using Zilon.Logic.Tactics;

class CombatWorldVM : MonoBehaviour
{

    public CombatMapVM Map;
    public SchemeLocator SchemeLocator;
    public Text Text;

    [Inject]
    public ICommandManager CommandManager;
    [Inject]
    public ICombatService CombatService;

    private Combat combat;

    private void FixedUpdate()
    {
        ExecuteCommands();
    }

    private void ExecuteCommands()
    {
        var command = CommandManager.Pop();
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
        var combat = CombatService.CreateCombat(initData);
        Map.InitCombat(combat);

        this.combat = combat;
    }
}
