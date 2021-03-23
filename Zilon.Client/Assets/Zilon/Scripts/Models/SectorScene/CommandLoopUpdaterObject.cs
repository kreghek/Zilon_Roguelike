using System.Threading;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Client.Sector;

public class CommandLoopUpdaterObject : MonoBehaviour
{
    [Inject]
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

    private void CommandLoopUpdater_ErrorOccured(object sender, ErrorOccuredEventArgs e)
    {
        Debug.LogError(e.Exception);
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
