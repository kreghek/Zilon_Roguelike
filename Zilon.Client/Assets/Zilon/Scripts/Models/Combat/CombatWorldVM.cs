using Assets.Zilon.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

class CombatWorldVM : MonoBehaviour
{

    public CombatMapVM Map;
    public SchemeLocator SchemeLocator;
    public Text Text;

    private ICommandManager commandManager;

    // Use this for initialization
    void Start()
    {
        commandManager = new CombatCommandManager();
        Map.SetCommandManager(commandManager);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        var command = commandManager.Pop();
        if (command == null)
            return;

        command.Execute();
    }
}
