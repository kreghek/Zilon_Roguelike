using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на использование предмета.
    /// </summary>
    public class UseSelfCommand : ActorCommandBase
    {
        private readonly IPlayer _player;
        private readonly IInventoryState _inventoryState;

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
            var propVm = _inventoryState.SelectedProp;
            if (propVm == null)
            {
                return false;
            }

            var prop = propVm.Prop;
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
            var propVm = _inventoryState.SelectedProp;
            var usableProp = propVm.Prop;

            var taskContext = new ActorTaskContext(_player.SectorNode.Sector, _player.Globe);

            var intention = new Intention<UsePropTask>(actor => new UsePropTask(actor, taskContext, usableProp));
            PlayerState.TaskSource.Intent(intention, PlayerState.ActiveActor.Actor);
        }
    }
}