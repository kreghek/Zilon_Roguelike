﻿using System;
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
        private readonly List<IMapNode> _path;
        private readonly IActorManager _actorManager;

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
            IPlayerState playerState,
            IActorManager actorManager) :
            base(gameLoop, sectorManager, playerState)
        {
            _actorManager = actorManager;

            _path = new List<IMapNode>();
        }

        /// <summary>
        /// Определяем, может ли команда выполниться.
        /// </summary>
        /// <returns> Возвращает true, если перемещение возможно. Иначе, false. </returns>
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

        /// <summary>
        /// Проверяет, может ли команда совершить очередное перемещение по уже найденному пути.
        /// </summary>
        /// <returns> Возвращает true, если команду можно повторить. </returns>
        public bool CanRepeat()
        {
            var canRepeat = CanExecute() && CheckEnemies();
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

                var isAvailable = MapHelper.CheckNodeAvailability(SectorManager.CurrentSector.Map,
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