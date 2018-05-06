using Assets.Zilon.Scripts.Commands;
using Assets.Zilon.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

public class CombatWorldVM : MonoBehaviour
{

    public CombatMapVM Map;
    public SchemeLocator SchemeLocator;
    public Text Text;

    private readonly ICommandManager commandManager;

    // Use this for initialization
    void Start()
    {

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
