using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics.Behaviour;

public class SectorCommandContextFactory : MonoBehaviour
{
    [Inject]
    private readonly ISectorUiState _sectorUiState;

    [Inject]
    private readonly IHumanActorTaskSource _humanActorTaskSource;

    public SectorViewModel SectorViewModel;

    public SectorCommandContext CreateContext() {
        var context = new SectorCommandContext(
            SectorViewModel.Sector,
            _sectorUiState.ActiveActor,
            _humanActorTaskSource,
            _sectorUiState.HoverViewModel,
            _sectorUiState.SelectedViewModel);
        return context;
    }
}
