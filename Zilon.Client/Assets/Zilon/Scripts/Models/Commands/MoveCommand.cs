using Assets.Zilon.Scripts.Models.SectorScene;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    class MoveCommand : ActorCommandBase
    {

        public MoveCommand(ISectorManager sectorManager,
            IPlayerState playerState) : 
            base(sectorManager, playerState)
        {
            
        }

        public override bool CanExecute()
        {
            //TODO Здесь должна быть проверка
            // 1. Может ли текущий актёр ходить.
            // 2. Проходима ли ячейка.
            // 3. Свободна ли ячейка.
            // 4. Видит ли актёр указанную ячейку.
            // 5. Возможно ли выполнение каких-либо команд над актёрами
            // (Нельзя, если ещё выполняется текущая команда. Например, анимация перемещения.)
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            var sector = _sectorManager.CurrentSector;
            var selectedNodeVM = _playerState.SelectedNode;
            
            var targetNode = selectedNodeVM.Node;
            _playerState.TaskSource.IntentMove(targetNode);
            sector.Update();
        }
    }
}