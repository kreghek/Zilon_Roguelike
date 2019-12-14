using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

public class ActorModalCommandContextFactory : MonoBehaviour
{
    [Inject]
    private readonly ISectorUiState _sectorUiState;

    public ActorModalCommandContext CreateContext() {
        var context = new ActorModalCommandContext(
            _sectorUiState.ActiveActor.Actor,
            _sectorUiState.HoverViewModel,
            _sectorUiState.SelectedViewModel);
        return context;
    }
}
