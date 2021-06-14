using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на использование предмета.
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

        public override CanExecuteCheckResult CanExecute()
        {
            var propVm = _inventoryState.SelectedProp;
            if (propVm == null)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
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

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var taskContext = new ActorTaskContext(sector);
            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var isAllowed = UsePropHelper.CheckPropAllowedByRestrictions(prop, actor, taskContext);
            if (!isAllowed)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            return new CanExecuteCheckResult { IsSuccess = true };
        }

        protected override void ExecuteTacticCommand()
        {
            var propVm = _inventoryState.SelectedProp;
            var usableProp = propVm?.Prop;
            if (usableProp is null)
            {
                throw new InvalidOperationException();
            }

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var taskContext = new ActorTaskContext(sector);

            var intention = new Intention<UsePropTask>(actor => new UsePropTask(actor, taskContext, usableProp));
            var taskSource = PlayerState?.TaskSource;
            if (taskSource is null)
            {
                throw new InvalidOperationException();
            }

            var actor = PlayerState?.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            taskSource.Intent(intention, actor);
        }
    }
}