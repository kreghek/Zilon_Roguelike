using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на перемещение предметов из контейнера в инвентарь.
    /// </summary>
    public class TakeFromContainerTask : ContainerTaskBase
    {
        public TakeFromContainerTask(IActor actor,
            IPropContainer container,
            IEnumerable<IProp> props,
            IPropStore inventory) : base(actor, container, props, inventory)
        {
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
