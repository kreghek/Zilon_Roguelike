using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Graphs;
using Zilon.Core.PathFinding;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    public class MoveCommand : ActorCommandBase, IRepeatableCommand
    {
        private readonly IPlayer _player;

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
        public MoveCommand(
            IPlayer player,
            ISectorUiState playerState) :
            base(playerState)
        {
            if (playerState is null)
            {
                throw new ArgumentNullException(nameof(playerState));
            }

            Path = new List<IGraphNode>();

            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        /// <summary>
        /// Текущий путь, по которому будет перемещаться персонаж.
        /// </summary>
        public IList<IGraphNode> Path { get; }

        /// <summary>
        /// Выполнение команды на перемещение и обновление игрового цикла.
        /// </summary>
        protected override void ExecuteTacticCommand()
        {
            var selectedNodeVm = GetSelectedNodeViewModel();
            if (selectedNodeVm == null)
            {
                throw new InvalidOperationException(
                    "Невозможно выполнить команду на перемещение, если не указан целевой узел.");
            }

            CreatePath(selectedNodeVm);

            var targetNode = selectedNodeVm.Node;

            var currentSector = _player.SectorNode.Sector;

            var moveIntetion = new MoveIntention(targetNode, currentSector);
            PlayerState.TaskSource.Intent(moveIntetion, PlayerState.ActiveActor.Actor);
        }

        private bool CanExecuteForSelected()
        {
            var nodeViewModel = GetSelectedNodeViewModel();
            if (nodeViewModel is null)
            {
                return false;
            }

            if (PlayerState.ActiveActor?.Actor is null)
            {
                return false;
            }

            //test

            CreatePath(nodeViewModel);
            return Path.Any();
        }

        private bool CheckEnemies()
        {
            var actor = PlayerState.ActiveActor.Actor;
            var enemies = _player.SectorNode.Sector.ActorManager.Items
                .Where(x => (x != actor) && (x.Person.Fraction != actor.Person.Fraction)).ToArray();

            foreach (var enemyActor in enemies)
            {
                var distance = ((HexNode)actor.Node).CubeCoords.DistanceTo(((HexNode)enemyActor.Node).CubeCoords);

                if (distance > 5)
                {
                    continue;
                }

                var isAvailable = _player.SectorNode.Sector.Map.TargetIsOnLine(
                    actor.Node,
                    enemyActor.Node);

                if (isAvailable)
                {
                    return false;
                }
            }

            return true;
        }

        private void CreatePath(IMapNodeViewModel targetNode)
        {
            var actor = PlayerState.ActiveActor.Actor;
            var startNode = actor.Node;
            var finishNode = targetNode.Node;
            var map = _player.SectorNode.Sector.Map;

            Path.Clear();

            if (!map.IsPositionAvailableFor(finishNode, actor))
            {
                return;
            }

            var context = new ActorPathFindingContext(actor, map, finishNode);

            var astar = new AStar(context, startNode, finishNode);
            var resultState = astar.Run();
            if (resultState != State.GoalFound)
            {
                return;
            }

            RememberFoundPath(astar);
        }

        private IMapNodeViewModel GetHoverNodeViewModel()
        {
            return PlayerState.HoverViewModel as IMapNodeViewModel;
        }

        private IMapNodeViewModel GetSelectedNodeViewModel()
        {
            return PlayerState.SelectedViewModel as IMapNodeViewModel;
        }

        private void RememberFoundPath(AStar astar)
        {
            var foundPath = astar.GetPath().Skip(1).ToArray();
            foreach (var pathNode in foundPath)
            {
                Path.Add(pathNode);
            }
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
                return false;
            }

            if (!CanExecuteForSelected())
            {
                return false;
            }

            CreatePath(nodeViewModel);
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
    }
}