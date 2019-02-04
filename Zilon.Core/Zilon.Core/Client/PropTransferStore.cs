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
        /// <summary>
        /// Список добавленых в хранилище предметов по сравнению с базовым хранилищем.
        /// </summary>
        public List<IProp> PropAdded { get; }

        /// <summary>
        /// Список удалённых из хранилища предметов по сравнению с базовым хранилищем.
        /// </summary>
        public List<IProp> PropRemoved { get; }

        /// <summary>
        /// Базовое хранилище.
        /// </summary>
        public IPropStore PropStore { get; }

        /// <summary>
        /// Событие выстреливает, когда в хранилище появляется новый предмет.
        /// </summary>
        /// <remarks>
        /// Это событие не срабатывает, если изменилось количество ресурсов.
        /// </remarks>
        public event EventHandler<PropStoreEventArgs> Added;

        /// <summary>
        /// Событие выстреливает, если какой-либо предмет удалён из хранилища.
        /// </summary>
        public event EventHandler<PropStoreEventArgs> Removed;

        /// <summary>
        /// Событие выстреливает, когда один из предметов в хранилище изменяется.
        /// </summary>
        /// <remarks>
        /// Используется, когда изменяется количество ресурсов в стаке.
        /// </remarks>
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
        /// Предметы в хранилище.
        /// </summary>
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

        /// <summary>
        /// Добавление предмета в хранилище.
        /// </summary>
        /// <param name="prop"> Целевой предмет. </param>
        public void Add(IProp prop)
        {
            switch (prop)
            {
                case Resource resource:
                    TransferResource(resource, PropRemoved, PropAdded,
                        bittenEventHandler: Removed,
                        oppositEventHandler: Added);

                    break;

                case Equipment _:
                case Concept _:
                    TransferNoCount(prop, PropRemoved, PropAdded,
                        eventHandler: Added);
                    break;
            }
        }

        /// <summary>
        /// Удаление предмета из хранилища.
        /// </summary>
        /// <param name="prop"> Целевой предмет. </param>
        public void Remove(IProp prop)
        {
            switch (prop)
            {
                case Resource resource:
                    TransferResource(resource, PropAdded, PropRemoved,
                        bittenEventHandler: Added,
                        oppositEventHandler: Removed);
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
            EventHandler<PropStoreEventArgs> bittenEventHandler,
            EventHandler<PropStoreEventArgs> oppositEventHandler)
        {
            var bittenResource = bittenList.OfType<Resource>().SingleOrDefault(x => x.Scheme == resource.Scheme);
            if (bittenResource != null)
            {
                var bittenRemains = bittenResource.Count - resource.Count;
                if (bittenRemains > 0)
                {
                    Changed?.Invoke(this, new PropStoreEventArgs(resource));
                    return;
                }

                bittenList.Remove(bittenResource);

                if (bittenRemains < 0)
                {
                    //resource.Count - похоже на ошибку. Нужно добавить тест.
                    var oppositeResource = new Resource(resource.Scheme, resource.Count );
                    oppositList.Add(oppositeResource);
                }
            }
            else
            {
                var oppositeResource = oppositList.OfType<Resource>().SingleOrDefault(x => x.Scheme == resource.Scheme);
                if (oppositeResource == null)
                {
                    oppositeResource = new Resource(resource.Scheme, resource.Count);
                    oppositList.Add(oppositeResource);

                    // события
                    // раньше в инвентаре не было, а потом стало
                    var oldProp = PropStore.CalcActualItems().SingleOrDefault(x => x.Scheme == resource.Scheme);
                    if (oldProp == null)
                    {
                        oppositEventHandler?.Invoke(this, new PropStoreEventArgs(resource));
                    }
                    else
                    {
                        Changed?.Invoke(this, new PropStoreEventArgs(oldProp));
                    }
                }
                else
                {
                    oppositeResource.Count += resource.Count;
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
