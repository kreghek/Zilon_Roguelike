using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Props;

namespace Zilon.Core.Client
{
    /// <summary>
    /// Хранилище, используемое для трансфера предметов.
    /// </summary>
    public class PropTransferStore : IPropStore
    {

        public List<IProp> PropAdded { get; }

        public List<IProp> PropRemoved { get; }

        public IPropStore PropStore { get; }

        public event EventHandler<PropStoreEventArgs> Added;
        public event EventHandler<PropStoreEventArgs> Removed;
        public event EventHandler<PropStoreEventArgs> Changed;

        /// <summary>
        /// Конструктор для хранилища-трансфера.
        /// </summary>
        /// <param name="propStore"> Реальное хранилище предметов. </param>
        public PropTransferStore(IPropStore propStore)
        {
            PropStore = propStore;

            PropAdded = new List<IProp>();
            PropRemoved = new List<IProp>();
        }

        /// <summary>
        /// Возвращает текущий список предметов в хранилище.
        /// </summary>
        /// <returns></returns>
        public IProp[] CalcActualItems()
        {
            var result = new List<IProp>();
            var propStoreItems = PropStore.CalcActualItems();

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

                    case Equipment _:
                    case Concept _:
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
                switch (prop)
                {
                    case Resource _:
                        var existsResource = result.SingleOrDefault(x => x.Scheme == prop.Scheme);
                        if (existsResource == null)
                        {
                            result.Add(prop);
                        }
                        break;

                    case Equipment _:
                    case Concept _:
                        result.Add(prop);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }

            return result.ToArray();
        }

        public void Add(IProp prop)
        {
            switch (prop)
            {
                case Resource resource:
                    TransferResource(resource, PropRemoved, PropAdded,
                        eventHandler: Added);
                    break;

                case Equipment _:
                case Concept _:
                    TransferNoCount(prop, PropRemoved, PropAdded,
                        eventHandler: Added);
                    break;
            }
        }

        public void Remove(IProp prop)
        {
            switch (prop)
            {
                case Resource resource:
                    TransferResource(resource, PropAdded, PropRemoved,
                        eventHandler: Removed);

                    //var currentStoreProps = PropStore.CalcActualItems();
                    //var currentStoreProp = (Resource)currentStoreProps.SingleOrDefault(x => x.Scheme == resource.Scheme);

                    //var removedProp = (Resource)PropRemoved.SingleOrDefault(x => x.Scheme == resource.Scheme);
                    //if (removedProp?.Count >= currentStoreProp?.Count)
                    //{
                    //    Removed?.Invoke(this, new PropStoreEventArgs(resource));
                    //}
                    //else
                    //{
                    //    Changed?.Invoke(this, new PropStoreEventArgs(resource));
                    //}

                    break;

                case Equipment _:
                case Concept _:
                    TransferNoCount(prop, PropAdded, PropRemoved,
                        eventHandler: Removed);
                    break;
                default:
                    throw new ArgumentException($"Предмет неизвестного типа {prop.GetType()}.");
            }
        }

        private void TransferResource(Resource resource,
            IList<IProp> bittenList,
            IList<IProp> oppositList,
            EventHandler<PropStoreEventArgs> eventHandler)
        {
            var removedResource = bittenList.OfType<Resource>().SingleOrDefault(x => x.Scheme == resource.Scheme);
            if (removedResource != null)
            {
                var removedRemains = removedResource.Count - resource.Count;
                if (removedRemains > 0)
                {
                    return;
                }

                bittenList.Remove(removedResource);

                if (removedRemains < 0)
                {
                    var addedResource = new Resource(resource.Scheme, resource.Count);
                    oppositList.Add(addedResource);
                }
            }
            else
            {
                var addedResource = oppositList.OfType<Resource>().SingleOrDefault(x => x.Scheme == resource.Scheme);
                if (addedResource == null)
                {
                    addedResource = new Resource(resource.Scheme, resource.Count);
                    oppositList.Add(addedResource);

                    //var currentStoreProps = PropStore.CalcActualItems();
                    //var currentStoreProp = currentStoreProps.SingleOrDefault(x => x.Scheme == resource.Scheme);
                    //if (currentStoreProp == null || eventHandler == Removed)
                    //{
                    //    eventHandler?.Invoke(this, new PropStoreEventArgs(resource));
                    //}
                    //else
                    //{
                    //    Changed?.Invoke(this, new PropStoreEventArgs(resource));
                    //}
                }
                else
                {
                    addedResource.Count += resource.Count;
                    Changed?.Invoke(this, new PropStoreEventArgs(resource));
                }
            }
        }

        private void TransferNoCount(IProp prop,
            IList<IProp> bittenList,
            IList<IProp> oppositList,
            EventHandler<PropStoreEventArgs> eventHandler)
        {
            var isBitten = bittenList.Contains(prop);
            if (isBitten)
            {
                bittenList.Remove(prop);
            }

            oppositList.Add(prop);
            eventHandler?.Invoke(this, new PropStoreEventArgs(prop));
        }
    }
}
