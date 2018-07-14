using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на выбрасывание предметов из инвентаря.
    /// </summary>
    public class DropFromInventoryTask : ActorTaskBase
    {
        private readonly IEnumerable<IProp> _props;
        private readonly IPropStore _inventory;
        private readonly IPropContainerManager _containerManager;

        public DropFromInventoryTask(IActor actor,
            IEnumerable<IProp> props,
            IPropStore inventory,
            IPropContainerManager containerManager) : base(actor)
        {
            _props = props;
            _inventory = inventory;
            _containerManager = containerManager;
        }

        public override void Execute()
        {
            var newContainer = new FixedPropContainer(Actor.Node, _props.ToArray());
            _containerManager.Add(newContainer);

            foreach (var prop in _props)
            {
                _inventory.Remove(prop);
            }

            IsComplete = true;
        }
    }
}
