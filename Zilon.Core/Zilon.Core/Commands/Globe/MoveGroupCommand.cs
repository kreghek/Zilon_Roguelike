using System.Diagnostics;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Scoring;
using Zilon.Core.World;

namespace Zilon.Core.Commands.Globe
{
    public class MoveGroupCommand : ICommand
    {
        private const int TRAVEL_TURNS = 50;
        private readonly HumanPlayer _player;
        private readonly IWorldManager _worldManager;
        private readonly IGlobeUiState _globeUiState;

        /// <summary>
        /// Менеджер по работе с очками игрока.
        /// </summary>
        public IScoreManager ScoreManager { get; set; }

        public MoveGroupCommand(
            HumanPlayer player,
            IWorldManager worldManager,
            IGlobeUiState globeUiState)
        {
            _player = player;
            _worldManager = worldManager;
            _globeUiState = globeUiState;
        }

        /// <summary>Проверяет, возможно ли выполнение команды.</summary>
        /// <returns>
        /// Возвращает true, если команду можно выполнить. Иначе возвращает false.
        /// </returns>
        public bool CanExecute()
        {
            if (_player.MainPerson == null)
            {
                throw new CommandCantExecuteException();
            }

            var effects = _player.MainPerson.Effects;
            var hasSurvivalHazardEffects = effects.Items.OfType<SurvivalStatHazardEffect>()
                .Where(x=>x.Level >= SurvivalStatHazardLevel.Max);

            var hasSurvivalHazards = hasSurvivalHazardEffects.Any();

            return !hasSurvivalHazards;
        }

        /// <summary>Выполнение команды.</summary>
        public void Execute()
        {
            //var selectedNodeViewModel = (IGlobeNodeViewModel)_globeUiState.SelectedViewModel;

            //var currentNode = _player.GlobeNode;
            //var currentGlobeCell = _player.Terrain;
            //var region = _worldManager.Regions[currentGlobeCell];

            //if (region == selectedNodeViewModel.ParentRegion)
            //{
            //    // Значит путешествие внутри одной провинции.

            //    var neighborNodes = region.GetNext(currentNode);
            //    var selectedIsNeighbor = neighborNodes.Contains(selectedNodeViewModel.Node);

            //    if (selectedIsNeighbor)
            //    {
            //        var globeNode = selectedNodeViewModel.Node;


            //        if (globeNode.Scheme.SectorLevels != null || _player.GlobeNode.IsTown)
            //        {
            //            ScoreManager?.CountPlace(globeNode);
            //        }

            //        _player.GlobeNode = globeNode;

            //        // Обновление состояния разведки узлов провинции
            //        globeNode.ObservedState = GlobeNodeObservedState.Visited;

            //        UpdateSurvivals();
            //    }
            //}
            //else
            //{
            //    Debug.Assert(selectedNodeViewModel.ParentRegion == null, "Для узла должна быть задана провинция.");

            //    var currentTerrainNode = _player.GlobeNode;
            //    var currentTerrainCell = _player.Terrain;
            //    //TODO Выборку ячейки мира по узлу провиции нужно упростить.
            //    var targetNeighborTerrainCell = _worldManager.Regions.Single(x => x.Value == selectedNodeViewModel.ParentRegion).Key;
            //    var targetNeighborBorders = selectedNodeViewModel.ParentRegion.Nodes.OfType<GlobeRegionNode>().Where(node => node.IsBorder);
            //    var transitionNodes = RegionTransitionHelper.GetNeighborBorderNodes(currentTerrainNode,
            //                                                                        currentTerrainCell,
            //                                                                        targetNeighborBorders,
            //                                                                        targetNeighborTerrainCell);
            //    if (transitionNodes.Contains(selectedNodeViewModel.Node))
            //    {
            //        _player.GlobeNode = selectedNodeViewModel.Node;
            //        _player.Terrain = targetNeighborTerrainCell;

            //        // Обновление состояния разведки узлов провинции
            //        selectedNodeViewModel.Node.ObservedState = GlobeNodeObservedState.Visited;

            //        UpdateSurvivals();
            //    }
            //}
        }

        private void UpdateSurvivals()
        {
            for (var i = 0; i < TRAVEL_TURNS; i++)
            {
                _player.MainPerson.Survival?.Update();
            }
        }
    }
}
