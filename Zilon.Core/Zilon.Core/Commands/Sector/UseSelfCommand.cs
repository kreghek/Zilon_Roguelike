using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    ///     Команда на использование предмета.
    /// </summary>
    public class UseSelfCommand : ActorCommandBase
    {
        private readonly IInventoryState _inventoryState;
        private readonly IPlayer _player;

        [ExcludeFromCodeCoverage]
        public UseSelfCommand(
            IPlayer player,
            ISectorUiState playerState,
            IInventoryState inventoryState) :
            base(playerState)
        {
            _player = player;
            _inventoryState = inventoryState;
        }

        public override bool CanExecute()
        {
            IPropItemViewModel propVm = _inventoryState.SelectedProp;
            if (propVm == null)
            {
                return false;
            }

            IProp prop = propVm.Prop;
            if (prop == null)
            {
                throw new AppException("Для модели представления не задан предмет.");
            }

            if (prop.Scheme.Use == null)
            {
                throw new AppException("Попытка использовать предмет, для которого нет информации об использовании.");
            }

            // На использование лагеря отдельная логика.
            // Отдыхать можно только есть в секторе не осталось монстров.
            if (prop.Scheme.Sid == "camp-tools")
            {
                var enemiesInSector = _player.SectorNode.Sector.ActorManager.Items.Where(x => x != CurrentActor);
                if (enemiesInSector.Any())
                {
                    return false;
                }
            }

            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            IPropItemViewModel propVm = _inventoryState.SelectedProp;
            IProp usableProp = propVm.Prop;

            ActorTaskContext taskContext = new ActorTaskContext(_player.SectorNode.Sector);

            Intention<UsePropTask> intention =
                new Intention<UsePropTask>(actor => new UsePropTask(actor, taskContext, usableProp));
            PlayerState.TaskSource.Intent(intention, PlayerState.ActiveActor.Actor);
        }
    }
}