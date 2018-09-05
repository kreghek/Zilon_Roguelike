using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    public class AttackCommand : ActorCommandBase
    {
        private readonly ITacticalActUsageService _tacticalActUsageService;

        public AttackCommand(IGameLoop gameLoop,
            ISectorManager sectorManager,
            IPlayerState playerState,
            ITacticalActUsageService tacticalActUsageService) :
            base(gameLoop, sectorManager, playerState)
        {
            _tacticalActUsageService = tacticalActUsageService;
        }

        public override bool CanExecute()
        {
            var map = _sectorManager.CurrentSector.Map;

            var currentNode = _playerState.ActiveActor.Actor.Node;

            var selectedActorViewModel = _playerState.HoverViewModel as IActorViewModel;
            if (selectedActorViewModel == null)
            {
                return false;
            }

            var targetNode = selectedActorViewModel.Actor.Node;

            var targetIsOnLine = MapHelper.CheckNodeAvailability(map, currentNode, targetNode);
            var act = _playerState.ActiveActor.Actor.Person.TacticalActCarrier.Acts.FirstOrDefault();
            var isInDistance = act.CheckDistance(((HexNode)currentNode).CubeCoords, ((HexNode)targetNode).CubeCoords);

            var canExecute = targetIsOnLine && isInDistance;

            //TODO Добавить проверку:
            // 1. Выбран ли вражеский юнит.
            // 2. Находится ли в пределах досягаемости оружия. (1) Проверяется.
            // 3. Видим ли текущим актёром. (0.5) Условная видимость по прямой.
            // 4. Способно ли оружие атаковать.
            // 5. Доступен ли целевой актёр для атаки.
            // 6. Возможно ли выполнение каких-либо команд над актёрами
            // (Нельзя, если ещё выполняется текущая команда. Например, анимация перемещения.)
            return canExecute;
        }

        protected override void ExecuteTacticCommand()
        {
            var targetActorViewModel = (IActorViewModel)_playerState.HoverViewModel;

            var targetActor = targetActorViewModel.Actor;
            var intention = new Intention<AttackTask>(a => new AttackTask(a, targetActor, _tacticalActUsageService));
            _playerState.TaskSource.Intent(intention);
        }
    }
}