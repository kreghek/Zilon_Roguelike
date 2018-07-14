using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на выбрасывание предметов из контейнера.
    /// </summary>
    public class DropFromContainerTask : ContainerTaskBase
    {
        private readonly IPropContainerManager _containerManager;

        public DropFromContainerTask(IActor actor,
            IPropContainer container,
            IEnumerable<IProp> props,
            IPropStore inventory,
            IPropContainerManager containerManager) : base(actor, container, props, inventory)
        {
            _containerManager = containerManager;
        }

        public override void Execute()
        {
            var newContainer = new FixedPropContainer(Actor.Node, _props.ToArray());
            _containerManager.Add(newContainer);

            foreach (var prop in _props)
            {
                _container.Inventory.Remove(prop);
            }

            IsComplete = true;
        }
    }
}
