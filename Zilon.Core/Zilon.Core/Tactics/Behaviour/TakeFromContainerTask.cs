using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    public class TakeFromContainerTask : ActorTaskBase
    {
        private readonly IPropContainer _container;
        private readonly IEnumerable<IProp> _props;
        private readonly IInventory _inventory;

        public TakeFromContainerTask(IActor actor,
            IPropContainer container,
            IEnumerable<IProp> props,
            IInventory inventory) : base(actor)
        {
            _container = container;
            _props = props;
            _inventory = inventory;
        }

        public override void Execute()
        {
            foreach (var prop in _props)
            {
                _inventory.Add(prop);
            }

            foreach (var prop in _props)
            {
                _container.Inventory.Remove(prop);
            }

            IsComplete = true;
        }
    }
}
