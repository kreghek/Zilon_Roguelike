using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    public class OneStepMoveCommand : SpecialActorCommandBase
    {
        private readonly IActorManager _actorManager;

        public StepDirection? Direction { get; set; }

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
        public OneStepMoveCommand(IGameLoop gameLoop,
            ISectorManager sectorManager,
            ISectorUiState playerState,
            IActorManager actorManager) :
            base(gameLoop, sectorManager, playerState)
        {
            _actorManager = actorManager;
        }

        /// <summary>
        /// Определяем, может ли команда выполниться.
        /// </summary>
        /// <returns> Возвращает true, если перемещение возможно. Иначе, false. </returns>
        public override bool CanExecute()
        {
            var targetNode = GetTargetNode();

            return targetNode != null;
        }

        private IMapNode GetTargetNode()
        {
            var node = PlayerState.ActiveActor.Actor.Node as HexNode;

            var neighborNodes = SectorManager.CurrentSector.Map.GetNext(node).OfType<HexNode>();
            var directions = HexHelper.GetOffsetClockwise();

            var stepDirectionIndex = (int)Direction.Value - 1;
            var targetCubeCoords = node.CubeCoords + directions[stepDirectionIndex];

            var targetNode = neighborNodes.SingleOrDefault(x => x.CubeCoords == targetCubeCoords);
            return targetNode;
        }

        /// <summary>
        /// Выполнение команды на перемещение и обновление игрового цикла.
        /// </summary>
        protected override void ExecuteTacticCommand()
        {
            var targetNode = GetTargetNode();
            var targetMap = SectorManager.CurrentSector.Map;

            var moveIntetion = new MoveIntention(targetNode, targetMap);
            PlayerState.TaskSource.Intent(moveIntetion);
        }      
    }
}