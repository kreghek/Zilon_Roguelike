using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    public abstract class ContainerTaskBase : ActorTaskBase
    {
        protected readonly IPropContainer _container;
        protected readonly IEnumerable<IProp> _props;
        protected readonly IInventory _inventory;

        public ContainerTaskBase(IActor actor,
            IPropContainer container,
            IEnumerable<IProp> props,
            IInventory inventory) : base(actor)
        {
            _container = container;
            _props = props;
            _inventory = inventory;
        }
    }
}
