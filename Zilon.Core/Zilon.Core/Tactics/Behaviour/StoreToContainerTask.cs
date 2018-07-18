using System.Collections.Generic;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на перемещние предметов из инвентаря в контейнер.
    /// </summary>
    public class StoreToContainerTask : ContainerTaskBase
    {
        public StoreToContainerTask(IActor actor,
            IPropContainer container,
            IEnumerable<IProp> props,
            IPropStore inventory) : base(actor, container, props, inventory)
        {
        }

        public override void Execute()
        {
            foreach (var prop in _props)
            {
                _container.Content.Add(prop);
            }

            foreach (var prop in _props)
            {
                _inventory.Remove(prop);
            }

            IsComplete = true;
        }
    }
}
