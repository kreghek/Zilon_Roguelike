using Assets.Zilon.Scripts.Models.SectorScene;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
    class AttackCommand : ActorCommandBase
    {
        public AttackCommand(ISectorManager sectorManager,
            IPlayerState playerState) :
            base(sectorManager, playerState)
        {
        }

        public override bool CanExecute()
        {
            //TODO Здесь должна быть проверка:
            // 1. Выбран ли вражеский юнит.
            // 2. Находится ли в пределах досягаемости оружия.
            // 3. Видим ли текущим актёром.
            // 4. Способно ли оружие атаковать.
            // 5. Доступен ли целевой актёр для атаки.
            // 6. Возможно ли выполнение каких-либо команд над актёрами
            // (Нельзя, если ещё выполняется текущая команда. Например, анимация перемещения.)
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            var sector = _sectorManager.CurrentSector;
            var selectedActorVM = _playerState.SelectedActor;

            var targetActor = selectedActorVM.Actor;
            _playerState.TaskSource.IntentAttack(targetActor);
            sector.Update();
        }
    }
}