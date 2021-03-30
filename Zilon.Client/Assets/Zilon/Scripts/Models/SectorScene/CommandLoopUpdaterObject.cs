using System;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

public class CommandLoopContextObject : MonoBehaviour, ICommandLoopContext
{
    private TaskCompletionSource<bool> _waitForUpdateTcs;

    [Inject]
    private readonly IPlayer _player;

    [Inject]
    private readonly IHumanActorTaskSource<ISectorTaskSourceContext> _humanActorTaskSource;

    public bool HasNextIteration
    {
        get
        {
            var mainPerson = _player.MainPerson;
            if (mainPerson is null)
            {
                throw new InvalidOperationException("The main person is not defined to process commands.");
            }

            var playerPersonSurvivalModule = mainPerson.GetModule<ISurvivalModule>();

            return !playerPersonSurvivalModule.IsDead;
        }
    }

    public Task WaitForUpdate(CancellationToken cancellationToken)
    {
        return _waitForUpdateTcs.Task;
    }

    public void Start()
    {
        _waitForUpdateTcs = new TaskCompletionSource<bool>();
    }

    public void Update()
    {
        if (_waitForUpdateTcs is null)
        {
            return;
        }

        if (_humanActorTaskSource.CanIntent())
        {
            _waitForUpdateTcs.SetResult(true);

            _waitForUpdateTcs = new TaskCompletionSource<bool>();
        }
    }
}

public class CommandLoopUpdaterObject : MonoBehaviour, ICommandLoopUpdater
{
    private TaskCompletionSource<bool> _startTcs;
    private bool _hasPendingCommand;
    private ICommand _lastCommand;

    [Inject]
    private readonly ICommandLoopContext _commandLoopContext;

    [Inject]
    private readonly ICommandPool _commandPool;

    [Inject]
    private readonly ISectorUiState _sectorUiState;

    [Inject]
    private readonly IInventoryState _inventoryState;

    public bool IsStarted { get; private set; }

    public event EventHandler<ErrorOccuredEventArgs> ErrorOccured;
    public event EventHandler CommandAutoExecuted;
    public event EventHandler CommandProcessed;

    public bool HasPendingCommands()
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        IsStarted = true;

        if (_startTcs != null)
        {
            return _startTcs.Task;
        }
        else
        {
            throw new InvalidOperationException("Call MonoBehaviour.Start() first.");
        }
    }

    public void Start()
    {
        _startTcs = new TaskCompletionSource<bool>();

        ErrorOccured += CommandLoopUpdaterObject_ErrorOccured;
        CommandProcessed += CommandLoopUpdater_CommandProcessed;
    }

    public void OnDestroy()
    {
        ErrorOccured -= CommandLoopUpdaterObject_ErrorOccured;
        CommandProcessed -= CommandLoopUpdater_CommandProcessed;
    }

    public void Update()
    {
        if (!IsStarted)
        {
            return;
        }

        if (!_commandLoopContext.HasNextIteration)
        {
            _startTcs.SetResult(true);
            return;
        }

        if (_commandLoopContext.WaitForUpdate(CancellationToken.None).Status != TaskStatus.RanToCompletion)
        {
            return;
        }

        var newLastCommand = ExecuteCommandsInner(_lastCommand);
        _lastCommand = newLastCommand;
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
                catch (InvalidOperationException)
                {
                    commandWithError = command;
                    throw;
                }

                if (command is IRepeatableCommand repeatableCommand)
                {
                    if (repeatableCommand.CanRepeat() && repeatableCommand.CanExecute())
                    {
                        _commandPool.Push(repeatableCommand);
                        CommandAutoExecuted?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        _hasPendingCommand = false;
                        CommandProcessed?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    _hasPendingCommand = false;
                    CommandProcessed?.Invoke(this, EventArgs.Empty);
                }

                newLastCommand = command;
            }
            else
            {
                _hasPendingCommand = false;

                if (lastCommand != null)
                {
                    CommandProcessed?.Invoke(this, EventArgs.Empty);
                    newLastCommand = null;
                }
            }
        }
        catch (InvalidOperationException exception)
        {
            errorOccured = true;

            if (commandWithError != null)
            {
                ErrorOccured?.Invoke(this, new CommandErrorOccuredEventArgs(commandWithError, exception));
            }
            else
            {
                ErrorOccured?.Invoke(this, new ErrorOccuredEventArgs(exception));
            }

            newLastCommand = null;
        }
        finally
        {
            if (errorOccured)
            {
                _hasPendingCommand = false;
                CommandProcessed?.Invoke(this, EventArgs.Empty);
                newLastCommand = null;
            }
        }

        return newLastCommand;
    }

    private void CommandLoopUpdaterObject_ErrorOccured(object sender, ErrorOccuredEventArgs e)
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


    private void CommandLoopUpdater_CommandProcessed(object sender, EventArgs e)
    {
        _inventoryState.SelectedProp = null;
        _sectorUiState.SelectedViewModel = null;
    }

    /*[Inject]
    private readonly ICommandLoopUpdater _commandLoopUpdater;

    [Inject]
    private readonly ISectorUiState _sectorUiState;

    [Inject]
    private readonly IInventoryState _inventoryState;

    // Start is called before the first frame update
    public void Start()
    {
        if (!_commandLoopUpdater.IsStarted)
        {
            _commandLoopUpdater.CommandProcessed += CommandLoopUpdater_CommandProcessed;
            _commandLoopUpdater.ErrorOccured += CommandLoopUpdater_ErrorOccured;

            _commandLoopUpdater.StartAsync(CancellationToken.None);
        }
    }
    */
}
