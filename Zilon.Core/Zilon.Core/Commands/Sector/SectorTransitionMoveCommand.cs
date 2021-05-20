﻿using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на перемещение группы через переход между секторами.
    /// </summary>
    public class SectorTransitionMoveCommand : ActorCommandBase
    {
        private readonly IPlayer _player;

        /// <summary>
        /// Конструктор на создание команды перемещения.
        /// </summary>
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
        [ExcludeFromCodeCoverage]
        public SectorTransitionMoveCommand(
            IPlayer player,
            ISectorUiState playerState) :
            base(playerState)
        {
            _player = player;
        }

        /// <summary>
        /// Определяем, может ли команда выполниться.
        /// </summary>
        /// <returns> Возвращает true, если перемещение возможно. Иначе, false. </returns>
        public override CanExecuteCheckResult CanExecute()
        {
            if (CurrentActor is null)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            if (_player.Globe is null || _player.MainPerson is null)
            {
                // We can't check transition if the globe and/or the main person equal null.

                throw new InvalidOperationException("Player object is not initialized.");
            }

            var actorNode = CurrentActor.Node;
            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var map = sector.Map;

            var detectedTransition = TransitionDetection.Detect(map.Transitions, new[] { actorNode });

            var actorOnTransition = detectedTransition != null;

            if (!actorOnTransition)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            return new CanExecuteCheckResult { IsSuccess = true };
        }

        /// <summary>
        /// Выполнение команды на перемещение и обновление игрового цикла.
        /// </summary>
        protected override void ExecuteTacticCommand()
        {
            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var taskContext = new ActorTaskContext(sector);
            var intention = new Intention<SectorTransitTask>(a => new SectorTransitTask(a, taskContext));
            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var taskSource = PlayerState.TaskSource;
            if (taskSource is null)
            {
                throw new InvalidOperationException();
            }

            taskSource.Intent(intention, actor);

            // Drop waiting because the globe must not wait the person which is not in a sector.
            taskSource.DropIntentionWaiting();
        }
    }
}