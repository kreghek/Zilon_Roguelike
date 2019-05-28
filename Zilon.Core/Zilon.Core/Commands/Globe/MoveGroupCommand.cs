using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.Commands.Globe
{
    public class MoveGroupCommand : ICommand
    {
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
            return true;
        }

        /// <summary>Выполнение команды.</summary>
        public void Execute()
        {
            var selectedNodeViewModel = (IGlobeNodeViewModel)_globeUiState.SelectedViewModel;

            var currentNode = _player.GlobeNode;
            var currentGlobeCell = _player.Terrain;
            var region = _worldManager.Regions[currentGlobeCell];

            var neighborNodes = region.GetNext(currentNode);
            var selectedIsNeighbor = neighborNodes.Contains(selectedNodeViewModel.Node);

            if (selectedIsNeighbor)
            {
                var globeNode = selectedNodeViewModel.Node;


                if (globeNode.Scheme.SectorLevels != null || _player.GlobeNode.IsTown)
                {
                    ScoreManager?.CountPlace(globeNode);
                }

                _player.GlobeNode = globeNode;

                for (var i = 0; i < 150; i++)
                {
                    _player.MainPerson.Survival.Update();
                }
            }
        }
    }
}
