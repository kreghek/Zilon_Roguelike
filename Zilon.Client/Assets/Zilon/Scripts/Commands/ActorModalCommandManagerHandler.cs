using System;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Commands;

public class ActorModalCommandManagerHandler : MonoBehaviour
{
    [NotNull] [Inject] private readonly ICommandManager<ActorModalCommandContext> _clientCommandManager;

    private bool _interuptCommands;

    public ActorModalCommandContextFactory ActorModalCommandContextFactory;

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

        var sectorCommandContext = ActorModalCommandContextFactory.CreateContext();

        try
        {
            if (command != null)
            {
                command.Execute(sectorCommandContext);

                if (_interuptCommands)
                {
                    return;
                }

                if (command is IRepeatableCommand<ActorModalCommandContext> repeatableCommand)
                {
                    if (repeatableCommand.CanRepeat(sectorCommandContext))
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
