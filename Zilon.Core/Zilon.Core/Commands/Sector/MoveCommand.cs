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
    public class MoveCommand : ActorCommandBase, IRepeatableCommand
    {
        private readonly IActorManager _actorManager;

        public List<IMapNode> Path { get; }

        /// <summary>
        /// Конструктор на создание команды перемещения.
        /// </summary>
        /// <param name="gameLoop"> Игровой цикл.
        /// Нужен для того, чтобы команда выполнила обновление игрового цикла
        /// после завершения перемещения персонажа. </param>
        /// <param name="sectorManager"> Менеджер сектора.
        /// Нужен для получения информации о секторе. </param>
        /// <param name="playerState"> Состояние игрока.
        /// Нужен для получения информации о текущем состоянии игрока. </param>
        /// <param name="actorManager"> Менеджер актёров.
        /// Нужен для отслеживания противников при автоперемещении. </param>
        [ExcludeFromCodeCoverage]
        public MoveCommand(IGameLoop gameLoop,
            ISectorManager sectorManager,
            ISectorUiState playerState,
            IActorManager actorManager) :
            base(gameLoop, sectorManager, playerState)
        {
            _actorManager = actorManager;

            Path = new List<IMapNode>();
        }

        /// <summary>
        /// Определяем, может ли команда выполниться.
        /// </summary>
        /// <returns> Возвращает true, если перемещение возможно. Иначе, false. </returns>
        public override bool CanExecute()
        {
            var nodeViewModel = GetHoverNodeViewModel();
            if (nodeViewModel == null)
            {
                throw new Exception();
                return false;
            }

            CreatePath(nodeViewModel);
            if (!Path.Any())
            {
                throw new Exception();
            }

            return Path.Any();
        }

        /// <summary>
        /// Проверяет, может ли команда совершить очередное перемещение по уже найденному пути.
        /// </summary>
        /// <returns> Возвращает true, если команду можно повторить. </returns>
        public bool CanRepeat()
        {
            var canRepeat = CanExecuteForSelected() && CheckEnemies();
            return canRepeat;
        }

        /// <summary>
        /// Выполнение команды на перемещение и обновление игрового цикла.
        /// </summary>
        protected override void ExecuteTacticCommand()
        {
            var selectedNodeVm = GetSelectedNodeViewModel();
            if (selectedNodeVm == null)
            {
                throw new InvalidOperationException("Невозможно выполнить команду на перемещение, если не указан целевой узел.");
            }

            CreatePath(selectedNodeVm);

            var targetNode = selectedNodeVm.Node;
            var targetMap = SectorManager.CurrentSector.Map;

            var moveIntetion = new MoveIntention(targetNode, targetMap);
            PlayerState.TaskSource.Intent(moveIntetion);
        }

        private IMapNodeViewModel GetHoverNodeViewModel()
        {
            return PlayerState.HoverViewModel as IMapNodeViewModel;
        }

        private IMapNodeViewModel GetSelectedNodeViewModel()
        {
            return PlayerState.SelectedViewModel as IMapNodeViewModel;
        }

        private bool CanExecuteForSelected()
        {
            var nodeViewModel = GetSelectedNodeViewModel();
            if (nodeViewModel == null)
            {
                return false;
            }

            CreatePath(nodeViewModel);
            return Path.Any();
        }

        private void CreatePath(IMapNodeViewModel targetNode)
        {
            var actor = PlayerState.ActiveActor.Actor;
            var startNode = actor.Node;
            var finishNode = targetNode.Node;
            var map = SectorManager.CurrentSector.Map;

            Path.Clear();

            if (!map.IsPositionAvailableFor(finishNode, actor))
            {
                return;
            }

            var context = new PathFindingContext(actor);

            var astar = new AStar(map, context, startNode, finishNode);
            var resultState = astar.Run();
            if (resultState != State.GoalFound)
            {
                return;
            }

            RememberFoundPath(astar);
        }

        private void RememberFoundPath(AStar astar)
        {
            var foundPath = astar.GetPath().Skip(1).ToArray();
            foreach (var pathNode in foundPath)
            {
                Path.Add((HexNode)pathNode);
            }
        }

        private bool CheckEnemies()
        {
            var actor = PlayerState.ActiveActor.Actor;
            var enemies = _actorManager.Items
                .Where(x => x != actor && x.Owner != actor.Owner).ToArray();

            foreach (var enemyActor in enemies)
            {
                var distance = ((HexNode)actor.Node).CubeCoords.DistanceTo(((HexNode)enemyActor.Node).CubeCoords);

                if (distance > 5)
                {
                    continue;
                }

                var isAvailable = SectorManager.CurrentSector.Map.TargetIsOnLine(
                    actor.Node,
                    enemyActor.Node);

                if (isAvailable)
                {
                    return false;
                }
            }

            return true;
        }
    }
}