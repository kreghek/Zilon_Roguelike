using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Graphs;
using Zilon.Core.PathFinding;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    public class MoveCommand : ActorCommandBase, IRepeatableCommand<SectorCommandContext>
    {
        private readonly IActorManager _actorManager;

        /// <summary>
        /// Текущий путь, по которому будет перемещаться персонаж.
        /// </summary>
        public IList<IGraphNode> Path { get; }

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
        public MoveCommand() :
            base()
        {
            Path = new List<IGraphNode>();
        }

        /// <summary>
        /// Определяем, может ли команда выполниться.
        /// </summary>
        /// <returns> Возвращает true, если перемещение возможно. Иначе, false. </returns>
        public override bool CanExecute(SectorCommandContext context)
        {
            var nodeViewModel = GetHoverNodeViewModel(context);
            if (nodeViewModel == null)
            {
                return false;
            }

            CreatePath(context, nodeViewModel);
            return Path.Any();
        }

        /// <summary>
        /// Проверяет, может ли команда совершить очередное перемещение по уже найденному пути.
        /// </summary>
        /// <returns> Возвращает true, если команду можно повторить. </returns>
        public bool CanRepeat(SectorCommandContext context)
        {
            var canRepeat = CanExecuteForSelected(context) && CheckEnemies(context);
            return canRepeat;
        }

        /// <summary>
        /// Выполнение команды на перемещение и обновление игрового цикла.
        /// </summary>
        protected override void ExecuteTacticCommand(SectorCommandContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var selectedNodeVm = GetSelectedNodeViewModel(context);
            if (selectedNodeVm == null)
            {
                throw new InvalidOperationException("Невозможно выполнить команду на перемещение, если не указан целевой узел.");
            }

            CreatePath(context, selectedNodeVm);

            var targetNode = selectedNodeVm.Node;
            var targetMap = context.Sector.Map;

            var moveIntetion = new MoveIntention(targetNode, targetMap);
            context.HumanActorTaskSource.Intent(moveIntetion);
        }

        private IMapNodeViewModel GetHoverNodeViewModel(SectorCommandContext context)
        {
            return context.HoverViewModel as IMapNodeViewModel;
        }

        private IMapNodeViewModel GetSelectedNodeViewModel(SectorCommandContext context)
        {
            return context.SelectedViewModel as IMapNodeViewModel;
        }

        private bool CanExecuteForSelected(SectorCommandContext context)
        {
            var nodeViewModel = GetSelectedNodeViewModel(context);
            if (nodeViewModel == null)
            {
                return false;
            }

            CreatePath(context, nodeViewModel);
            return Path.Any();
        }

        private void CreatePath(SectorCommandContext context, IMapNodeViewModel targetNode)
        {
            var actor = context.ActiveActorViewModel.Actor;
            var startNode = actor.Node;
            var finishNode = targetNode.Node;
            var map = context.Sector.Map;

            Path.Clear();

            if (!map.IsPositionAvailableFor(finishNode, actor))
            {
                return;
            }

            var pathFindingContext = new ActorPathFindingContext(actor, map);

            var astar = new AStar(pathFindingContext, startNode, finishNode);
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
                Path.Add(pathNode);
            }
        }

        private bool CheckEnemies(SectorCommandContext context)
        {
            var actor = context.ActiveActorViewModel.Actor;
            var enemies = _actorManager.Items
                .Where(x => x != actor && x.Owner != actor.Owner).ToArray();

            foreach (var enemyActor in enemies)
            {
                var distance = ((HexNode)actor.Node).CubeCoords.DistanceTo(((HexNode)enemyActor.Node).CubeCoords);

                if (distance > 5)
                {
                    continue;
                }

                var isAvailable = context.Sector.Map.TargetIsOnLine(
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