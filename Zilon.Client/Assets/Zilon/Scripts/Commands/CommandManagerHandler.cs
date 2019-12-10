using System;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Commands;

public class CommandManagerHandler : MonoBehaviour
{
    [NotNull] [Inject] private readonly ICommandManager _clientCommandManager;

    private bool _interuptCommands;

    public void Start()
    {
        _clientCommandManager.CommandPushed += CommandPushed;
    }

    private void CommandPushed(object sender, EventArgs e)
    {
        ExecuteCommands();
    }

    private void ExecuteCommands()
    {
        var command = _clientCommandManager.Pop();

        try
        {
            if (command != null)
            {
                command.Execute();

                if (_interuptCommands)
                {
                    return;
                }

                if (command is IRepeatableCommand repeatableCommand)
                {
                    if (repeatableCommand.CanRepeat())
                    {
                        _clientCommandManager.Push(repeatableCommand);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Не удалось выполнить команду {command}.", exception);
        }
    }
}
