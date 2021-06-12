using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Graphs;
using Zilon.Core.PathFinding;
using Zilon.Core.Players;
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
        /// <param name="gameLoop">
        /// Игровой цикл.
        /// Нужен для того, чтобы команда выполнила обновление игрового цикла
        /// после завершения перемещения персонажа.
        /// </param>
        /// <param name="sectorManager">
        /// Менеджер сектора.
        /// Нужен для получения информации о секторе.
        /// </param>
        /// <param name="playerState">
        /// Состояние игрока.
        /// Нужен для получения информации о текущем состоянии игрока.
        /// </param>
        /// <param name="actorManager">
        /// Менеджер актёров.
        /// Нужен для отслеживания противников при автоперемещении.
        /// </param>
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
            if (selectedNodeVm is null)
            {
                throw new InvalidOperationException(
                    "Невозможно выполнить команду на перемещение, если не указан целевой узел.");
            }

            CreatePath(selectedNodeVm);

            var targetNode = selectedNodeVm.Node;

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var currentSector = sector;

            var moveIntetion = new MoveIntention(targetNode, currentSector);
            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var taskSource = PlayerState?.TaskSource;
            if (taskSource is null)
            {
                throw new InvalidOperationException();
            }

            taskSource.Intent(moveIntetion, actor);
        }

        private bool CheckEnemies()
        {
            var actor = PlayerState?.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var enemies = sector.ActorManager.Items
                .Where(x => x != actor && x.Person.Fraction != actor.Person.Fraction).ToArray();

            foreach (var enemyActor in enemies)
            {
                var distance = ((HexNode)actor.Node).CubeCoords.DistanceTo(((HexNode)enemyActor.Node).CubeCoords);

                if (distance > 5)
                {
                    continue;
                }

                var isAvailable = sector.Map.TargetIsOnLine(
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
            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var startNode = actor.Node;
            var finishNode = targetNode.Node;

            if (startNode == finishNode)
            {
                Path.Clear();
                return;
            }

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var map = sector.Map;

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

        private IMapNodeViewModel? GetHoverNodeViewModel()
        {
            return PlayerState.HoverViewModel as IMapNodeViewModel;
        }

        private IMapNodeViewModel? GetSelectedNodeViewModel()
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

        /// <inheritdoc />
        public int RepeatIteration { get; private set; }

        /// <summary>
        /// Определяем, может ли команда выполниться.
        /// </summary>
        /// <returns> Возвращает true, если перемещение возможно. Иначе, false. </returns>
        public override CanExecuteCheckResult CanExecute()
        {
            var nodeViewModel = GetHoverNodeViewModel();
            if (nodeViewModel is null)
            {
                return CanExecuteCheckResult.CreateFailed("No hover node.");
            }

            if (PlayerState.ActiveActor?.Actor is null)
            {
                return CanExecuteCheckResult.CreateFailed("Active actor is not assigned.");
            }

            CreatePath(nodeViewModel);

            var pathIsNotEmpty = Path.Any();

            if (!pathIsNotEmpty)
            {
                return CanExecuteCheckResult.CreateFailed("Found path is not correct or empty.");
            }

            return CanExecuteCheckResult.CreateSuccessful();
        }

        /// <summary>
        /// Проверяет, может ли команда совершить очередное перемещение по уже найденному пути.
        /// </summary>
        /// <returns> Возвращает true, если команду можно повторить. </returns>
        public bool CanRepeat()
        {
            var canRepeat = CheckEnemies();
            return canRepeat;
        }

        /// <inheritdoc />
        public void IncreaceIteration()
        {
            RepeatIteration++;
        }
    }
}