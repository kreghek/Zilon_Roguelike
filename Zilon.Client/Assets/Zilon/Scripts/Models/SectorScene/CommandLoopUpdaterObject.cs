using System;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;

public class CommandLoopUpdaterObject : MonoBehaviour
{
    [Inject]
    private readonly ICommandLoopContext _commandLoopContext;

    [Inject]
    private readonly ICommandPool _commandPool;

    [Inject]
    private readonly ISectorUiState _sectorUiState;

    [Inject]
    private readonly IInventoryState _inventoryState;

    public void Update()
    {
        if (!_commandLoopContext.HasNextIteration)
        {
            return;
        }

        if (!_commandLoopContext.CanPlayerGiveCommand)
        {
            return;
        }

        var command = _commandPool.Pop();

        try
        {
            if (command != null)
            {
                command.Execute();

                if (command is IRepeatableCommand repeatableCommand)
                {
                    if (repeatableCommand.CanRepeat() && repeatableCommand.CanExecute())
                    {
                        _commandPool.Push(repeatableCommand);
                    }
                }
            }
        }
        catch(Exception exception)
        {
            Debug.LogError(exception);
        }
    }

    private void CommandLoopUpdater_CommandProcessed(object sender, System.EventArgs e)
    {
        _inventoryState.SelectedProp = null;
        _sectorUiState.SelectedViewModel = null;
    }
}
