using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    public class GiveToContainerTask : ActorTaskBase
    {
        private readonly IEnumerable<IProp> _props;
        private readonly IInventory _inventory;

        public GiveToContainerTask(IActor actor,
            IEnumerable<IProp> props,
            IInventory inventory) : base(actor)
        {
            _props = props;
            _inventory = inventory;
        }

        public override void Execute()
        {
            foreach (var prop in _props)
            {
                _inventory.Add(prop);
            }

            IsComplete = true;
        }
    }
}
