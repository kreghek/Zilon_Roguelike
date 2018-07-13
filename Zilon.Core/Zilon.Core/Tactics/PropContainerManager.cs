using System.Collections.Generic;

namespace Zilon.Core.Tactics
{
    public class PropContainerManager : IPropContainerManager
    {
        private readonly List<IPropContainer> _containers;

        public IEnumerable<IPropContainer> Containers => _containers;

        public void Add(IPropContainer propContainer)
        {
            _containers.Add(propContainer);
        }

        public void Add(IEnumerable<IPropContainer> propContainers)
        {
            _containers.AddRange(propContainers);
        }
    }
}
