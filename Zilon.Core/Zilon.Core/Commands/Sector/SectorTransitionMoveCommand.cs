using Zilon.Core.Client;
using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Commands
{
    /// <summary>
    ///     Команда на перемещение группы через переход между секторами.
    /// </summary>
    public class SectorTransitionMoveCommand : ActorCommandBase
    {
        private readonly IPlayer _player;

        /// <summary>
        ///     Конструктор на создание команды перемещения.
        /// </summary>
        /// Нужен для того, чтобы команда выполнила обновление игрового цикла
        /// после завершения перемещения персонажа.
        /// </param>
        /// <param name="sectorManager">
        ///     Менеджер сектора.
        ///     Нужен для получения информации о секторе.
        /// </param>
        /// <param name="playerState">
        ///     Состояние игрока.
        ///     Нужен для получения информации о текущем состоянии игрока.
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
        ///     Определяем, может ли команда выполниться.
        /// </summary>
        /// <returns> Возвращает true, если перемещение возможно. Иначе, false. </returns>
        public override bool CanExecute()
        {
            if (CurrentActor == null)
            {
                return false;
            }

            IGraphNode actorNode = CurrentActor.Node;
            ISectorMap map = _player.SectorNode.Sector.Map;

            RoomTransition detectedTransition = TransitionDetection.Detect(map.Transitions, new[] {actorNode});

            var actorOnTransition = detectedTransition != null;

            return actorOnTransition;
        }

        /// <summary>
        ///     Выполнение команды на перемещение и обновление игрового цикла.
        /// </summary>
        protected override void ExecuteTacticCommand()
        {
            ActorTaskContext taskContext = new ActorTaskContext(_player.SectorNode.Sector);
            Intention<SectorTransitTask> intention =
                new Intention<SectorTransitTask>(a => new SectorTransitTask(a, taskContext));
            PlayerState.TaskSource.Intent(intention, PlayerState.ActiveActor.Actor);
        }
    }
}