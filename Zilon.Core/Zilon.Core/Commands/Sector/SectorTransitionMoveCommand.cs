using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на перемещение группы через переход между секторами.
    /// </summary>
    public class SectorTransitionMoveCommand : ActorCommandBase
    {
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
            ISectorManager sectorManager,
            ISectorUiState playerState) :
            base(sectorManager, playerState)
        {
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
            var map = SectorManager.CurrentSector.Map;

            var detectedTransition = TransitionDetection.Detect(map.Transitions, new[] { actorNode });

            var actorOnTransition = detectedTransition != null;

            return actorOnTransition;
        }

        /// <summary>
        /// Выполнение команды на перемещение и обновление игрового цикла.
        /// </summary>
        protected override void ExecuteTacticCommand()
        {
            var actorNode = CurrentActor.Node;
            var map = SectorManager.CurrentSector.Map;

            var detectedTransition = TransitionDetection.Detect(map.Transitions, new[] { actorNode });

            SectorManager.CurrentSector.UseTransition(detectedTransition);
        }
    }
}