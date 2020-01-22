using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда на использование предмета.
    /// </summary>
    public class UseSelfCommand : ActorCommandBase
    {
        private readonly IInventoryState _inventoryState;

        [ExcludeFromCodeCoverage]
        public UseSelfCommand(
            IInventoryState inventoryState) :
            base()
        {
            _inventoryState = inventoryState;
        }

        public override bool CanExecute(SectorCommandContext context)
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

            return true;
        }

        protected override void ExecuteTacticCommand(SectorCommandContext context)
        {
            var propVm = _inventoryState.SelectedProp;
            var usableProp = propVm.Prop;

            var intention = new Intention<UsePropTask>(a => new UsePropTask(a, usableProp));
            context.TaskSource.Intent(intention);
        }
    }
}