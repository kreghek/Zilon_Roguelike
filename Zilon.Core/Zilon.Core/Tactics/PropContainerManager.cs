using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Tactics
{
    public class PropContainerManager : IPropContainerManager
    {
        private readonly List<IPropContainer> _containers;

        public event EventHandler<ManagerItemsChangedArgs<IPropContainer>> Added;
        public event EventHandler<ManagerItemsChangedArgs<IPropContainer>> Removed;

        public IEnumerable<IPropContainer> Containers => _containers;

        public PropContainerManager()
        {
            _containers = new List<IPropContainer>();
        }

        public void Add(IPropContainer propContainer)
        {
            _containers.Add(propContainer);
            DoAdded(propContainer);
        }

        public void Add(IEnumerable<IPropContainer> propContainers)
        {
            _containers.AddRange(propContainers);
            DoAdded(propContainers.ToArray());
        }

        public void Remove(IPropContainer propContainer)
        {
            _containers.Remove(propContainer);
            DoRemoved(propContainer);
        }

        public void Remove(IEnumerable<IPropContainer> propContainers)
        {
            foreach (var container in propContainers)
            {
                _containers.Remove(container);
            }
            DoRemoved(propContainers.ToArray());
        }

        private void DoAdded(params IPropContainer[] containers)
        {
            var args = new ManagerItemsChangedArgs<IPropContainer>(containers);
            Added?.Invoke(this, args);
        }

        private void DoRemoved(params IPropContainer[] containers)
        {
            var args = new ManagerItemsChangedArgs<IPropContainer>(containers);
            Removed?.Invoke(this, args);
        }
    }
}
