using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;

namespace Zilon.Core.Client
{
    public class PropTransferStore : IPropStore
    {
        private readonly IPropStore _propStore;

        public PropTransferStore(IPropStore propStore)
        {
            _propStore = propStore;

            PropAdded = new List<IProp>();
            PropRemoved = new List<IProp>();
        }

        public IProp[] CalcActualItems()
        {
                return CalcItems();
        }

        public List<IProp> PropAdded { get; }

        public List<IProp> PropRemoved { get; }

        private IProp[] CalcItems()
        {
            var result = new List<IProp>();
            var propStoreItems = _propStore.CalcActualItems();

            foreach (var prop in propStoreItems)
            {
                switch (prop)
                {
                    case Resource resource:
                        var removedResource = PropRemoved.OfType<Resource>()
                            .SingleOrDefault(x => x.Scheme == resource.Scheme);

                        var addedResource = PropAdded.OfType<Resource>()
                            .SingleOrDefault(x => x.Scheme == resource.Scheme);

                        var addedCount = addedResource?.Count;
                        var removedCount = removedResource?.Count;
                        var remainsCount = resource.Count + addedCount.GetValueOrDefault() - removedCount.GetValueOrDefault();

                        if (remainsCount > 0)
                        {
                            var remainsResource = new Resource(resource.Scheme, remainsCount);
                            result.Add(remainsResource);
                        }
                        
                        break;

                    case Equipment equipment:
                    case Recipe recipe:
                        var isRemoved = PropRemoved.Contains(prop);

                        if (!isRemoved)
                        {
                            result.Add(prop);
                        }
                        break;
                }
            }

            foreach (var prop in PropAdded)
            {
                result.Add(prop);
            }

            return result.ToArray();
        }

        public event EventHandler<PropStoreEventArgs> Added;
        public event EventHandler<PropStoreEventArgs> Removed;
        public event EventHandler<PropStoreEventArgs> Changed;

        public void Add(IProp prop)
        {
            switch (prop)
            {
                case Resource resource:

                    TransferResource(resource, PropRemoved, PropAdded);
                    break;

                case Equipment equipment:
                case Recipe recipe:
                    TransferNoCount(prop, PropRemoved, PropAdded);
                    break;
            }
        }

        private void TransferResource(Resource resource, IList<IProp> bittenList, IList<IProp> oppositList)
        {
            var removedResource = bittenList.OfType<Resource>().SingleOrDefault(x => x.Scheme == resource.Scheme);
            if (removedResource != null)
            {
                var removedRemains = removedResource.Count - resource.Count;
                if (removedRemains <= 0)
                {
                    bittenList.Remove(removedResource);

                    if (removedRemains < 0)
                    {
                        var addedResource = new Resource(resource.Scheme, resource.Count);
                        oppositList.Add(addedResource);
                    }
                }
            }
            else
            {
                var addedResource = oppositList.OfType<Resource>().SingleOrDefault(x => x.Scheme == resource.Scheme);
                if (addedResource == null)
                {
                    addedResource = new Resource(resource.Scheme, resource.Count);
                    oppositList.Add(addedResource);
                }
                else
                {
                    addedResource.Count += resource.Count;
                }
            }
        }

        public void Remove(IProp prop)
        {
            switch (prop)
            {
                case Resource resource:

                    TransferResource(resource, PropAdded, PropRemoved);
                    break;

                case Equipment equipment:
                case Recipe recipe:
                    TransferNoCount(prop, PropAdded, PropRemoved);
                    break;
            }
        }

        private void TransferNoCount(IProp prop, IList<IProp> bittenList, IList<IProp> oppositList)
        {
            var isRemoved = bittenList.Contains(prop);
            if (isRemoved)
            {
                bittenList.Remove(prop);
            }

            var propStoreItems = _propStore.CalcActualItems();
            var inOriginal = propStoreItems.Contains(prop);
            if (!inOriginal)
            {
                oppositList.Add(prop);
            }
        }
    }
}
