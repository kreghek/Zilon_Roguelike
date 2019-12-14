using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

public class SectorInteration : MonoBehaviour
{
    [NotNull] [Inject] private readonly ICommandManager<SectorCommandContext> _clientCommandExecutor;

    [NotNull] [Inject] private readonly ISectorUiState _playerUiState;

    [NotNull]
    [Inject(Id = "move-command")]
    private readonly ICommand<SectorCommandContext> _moveCommand;

    public SectorMapViewModel MapViewModel;

    public void Start()
    {
        MapViewModel.NodeSelected += MapViewModel_NodeSelected;
    }

    private void MapViewModel_NodeSelected(object sender, Assets.Zilon.Scripts.Models.Sector.NodeInteractEventArgs e)
    {
        _playerUiState.SelectedViewModel = e.NodeViewModel;

        if (e.NodeViewModel != null)
        {
            _clientCommandExecutor.Push(_moveCommand);
        }
    }
}
