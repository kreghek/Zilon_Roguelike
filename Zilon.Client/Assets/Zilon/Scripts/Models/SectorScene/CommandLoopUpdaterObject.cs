using System.Threading;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;

public class CommandLoopUpdaterObject : MonoBehaviour
{
    [Inject]
    private readonly ICommandLoopUpdater _commandLoopUpdater;

    [Inject]
    private readonly ICommandLoopContext _commandLoopContext;

    [Inject]
    private readonly ICommandPool _commandPool;

    [Inject]
    private readonly ISectorUiState _sectorUiState;

    [Inject]
    private readonly IInventoryState _inventoryState;
    private bool _hasPendingCommand;
    private ICommand lastCommand;

    //// Start is called before the first frame update
    //public void Start()
    //{
    //    if (!_commandLoopUpdater.IsStarted)
    //    {
    //        _commandLoopUpdater.CommandProcessed += CommandLoopUpdater_CommandProcessed;
    //        _commandLoopUpdater.ErrorOccured += CommandLoopUpdater_ErrorOccured;

    //        _commandLoopUpdater.StartAsync(CancellationToken.None);
    //    }
    //}

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

        try
        {
            lastCommand = ExecuteCommandsInner(lastCommand);
        }
        catch
        {

        }
    }

    private ICommand ExecuteCommandsInner(ICommand lastCommand)
    {
        ICommand commandWithError = null;
        ICommand newLastCommand = null;


        _hasPendingCommand = true;

        var errorOccured = false;

        var command = _commandPool.Pop();

        try
        {
            if (command != null)
            {
                try
                {
                    command.Execute();
                }
                catch
                {
                    commandWithError = command;
                    throw;
                }

                if (command is IRepeatableCommand repeatableCommand)
                {
                    if (repeatableCommand.CanRepeat() && repeatableCommand.CanExecute())
                    {
                        _commandPool.Push(repeatableCommand);
                    }
                    else
                    {
                        _hasPendingCommand = false;
                    }
                }
                else
                {
                    _hasPendingCommand = false;
                }

                newLastCommand = command;
            }
            else
            {
                _hasPendingCommand = false;

                if (lastCommand != null)
                {
                    newLastCommand = null;
                }
            }
        }
        catch
        {
            errorOccured = true;

            newLastCommand = null;
        }
        finally
        {
            if (errorOccured)
            {
                _hasPendingCommand = false;
                newLastCommand = null;
            }
        }


        return newLastCommand;
    }

    private void CommandLoopUpdater_ErrorOccured(object sender, ErrorOccuredEventArgs e)
    {
        if (e is CommandErrorOccuredEventArgs commandEventArgs)
        {
            Debug.LogError(commandEventArgs.Command.GetType());
            Debug.LogError(commandEventArgs.Exception);
        }
        else
        {
            Debug.LogError(e.Exception);
        }
    }

    public void OnDestroy()
    {
        _commandLoopUpdater.CommandProcessed -= CommandLoopUpdater_CommandProcessed;
        _commandLoopUpdater.ErrorOccured -= CommandLoopUpdater_ErrorOccured;
    }

    private void CommandLoopUpdater_CommandProcessed(object sender, System.EventArgs e)
    {
        _inventoryState.SelectedProp = null;
        _sectorUiState.SelectedViewModel = null;
    }
}
