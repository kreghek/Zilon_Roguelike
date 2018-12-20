using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tactics.Spatial.PathFinding;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    public class MoveCommand : ActorCommandBase
    {
        private readonly List<IMapNode> _path;

        [ExcludeFromCodeCoverage]
        public MoveCommand(IGameLoop gameLoop,
            ISectorManager sectorManager,
            IPlayerState playerState) :
            base(gameLoop, sectorManager, playerState)
        {
            _path = new List<IMapNode>();
        }

        public override bool CanExecute()
        {
            var nodeViewModel = GetSelectedNodeViewModel();
            if (nodeViewModel == null)
            {
                return false;
            }

            CreatePath();
            return _path.Any();
        }

        protected override void ExecuteTacticCommand()
        {
            var selectedNodeVm = GetSelectedNodeViewModel();
            if (selectedNodeVm == null)
            {
                throw new InvalidOperationException("Невозможно выполнить команду на перемещение, если не указан целевой узел.");
            }

            var targetNode = selectedNodeVm.Node;
            var targetMap = SectorManager.CurrentSector.Map;

            var moveIntetion = new MoveIntention(targetNode, targetMap);
            PlayerState.TaskSource.Intent(moveIntetion);
        }

        private IMapNodeViewModel GetSelectedNodeViewModel()
        {
            return PlayerState.HoverViewModel as IMapNodeViewModel;
        }

        private void CreatePath()
        {
            var nodeViewModel = GetSelectedNodeViewModel();

            var startNode = PlayerState.ActiveActor.Actor.Node;
            var finishNode = nodeViewModel.Node;
            var map = SectorManager.CurrentSector.Map;

            _path.Clear();

            var context = new PathFindingContext(PlayerState.ActiveActor.Actor);

            var astar = new AStar(map, context, startNode, finishNode);
            var resultState = astar.Run();
            if (resultState != State.GoalFound)
            {
                return;
            }

            var foundPath = astar.GetPath().Skip(1).ToArray();
            foreach (var pathNode in foundPath)
            {
                _path.Add((HexNode)pathNode);
            }
        }
    }
}