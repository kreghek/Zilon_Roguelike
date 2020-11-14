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
        /// после завершения перемещения персонажа. </param>
        /// <param name="sectorManager"> Менеджер сектора.
        /// Нужен для получения информации о секторе. </param>
        /// <param name="playerState"> Состояние игрока.
        /// Нужен для получения информации о текущем состоянии игрока. </param>
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
        public override bool CanExecute()
        {
            if (CurrentActor == null)
            {
                return false;
            }

            var actorNode = CurrentActor.Node;
            var map = _player.SectorNode.Sector.Map;

            var detectedTransition = TransitionDetection.Detect(map.Transitions, new[]
            {
                actorNode
            });

            var actorOnTransition = detectedTransition != null;

            return actorOnTransition;
        }

        /// <summary>
        /// Выполнение команды на перемещение и обновление игрового цикла.
        /// </summary>
        protected override void ExecuteTacticCommand()
        {
            var taskContext = new ActorTaskContext(_player.SectorNode.Sector);
            var intention = new Intention<SectorTransitTask>(a => new SectorTransitTask(a, taskContext));
            PlayerState.TaskSource.Intent(intention, PlayerState.ActiveActor.Actor);
        }
    }
}