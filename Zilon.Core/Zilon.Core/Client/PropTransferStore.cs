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

        private void MergeResource(List<IProp> result, Resource resource)
        {
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
        }

        private void ProcessAddedProps(List<IProp> result)
        {
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
        }

        private void ProcessCurrentProps(List<IProp> result, IProp[] propStoreItems)
        {
            foreach (var prop in propStoreItems)
            {
                switch (prop)
                {
                    case Resource resource:
                        MergeResource(result, resource);
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
        }

        private static void TransferNoCount(IProp prop,
            IList<IProp> bittenList,
            IList<IProp> oppositList)
        {
            var isBitten = bittenList.Contains(prop);
            if (isBitten)
            {
                bittenList.Remove(prop);
            }

            oppositList.Add(prop);
        }

        private static void TransferResource(Resource resource,
            IList<IProp> mainList,
            IList<IProp> oppositList)
        {
            var oppositResource = oppositList.OfType<Resource>().SingleOrDefault(x => x.Scheme == resource.Scheme);
            if (oppositResource != null)
            {
                var remains = oppositResource.Count - resource.Count;
                if (remains <= 0)
                {
                    oppositList.Remove(oppositResource);

                    if (remains < 0)
                    {
                        //resource.Count - похоже на ошибку. Нужно добавить тест.
                        // должно быть что-то типа remains * -1
                        // сейчас просто нет ситуации, когда у нас, например, из инвентаря было изъято 10 ед. А потом 15 добавлено.
                        var mainResource = new Resource(resource.Scheme, resource.Count);
                        mainList.Add(mainResource);
                    }
                }
            }
            else
            {
                var mainResource = mainList.OfType<Resource>().SingleOrDefault(x => x.Scheme == resource.Scheme);
                if (mainResource == null)
                {
                    mainResource = new Resource(resource.Scheme, resource.Count);
                    mainList.Add(mainResource);
                }
                else
                {
                    mainResource.Count += resource.Count;
                }
            }
        }

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
        /// Предметы в хранилище.
        /// </summary>
        public IProp[] CalcActualItems()
        {
            var result = new List<IProp>();
            var propStoreItems = PropStore.CalcActualItems();
            ProcessCurrentProps(result, propStoreItems);
            ProcessAddedProps(result);

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

                    // запоминаем предыдущее состояния для событий
                    var oldProp = (Resource)CalcActualItems()?.SingleOrDefault(x => x.Scheme == prop.Scheme);

                    TransferResource(resource, PropAdded, PropRemoved);

                    // Выброс событий
                    var currentProp = CalcActualItems()?.SingleOrDefault(x => x.Scheme == prop.Scheme);

                    if (oldProp == null)
                    {
                        Added?.Invoke(this, new PropStoreEventArgs(currentProp));
                    }
                    else
                    {
                        Changed?.Invoke(this, new PropStoreEventArgs(currentProp));
                    }

                    break;

                case Equipment _:
                case Concept _:
                    TransferNoCount(prop, PropRemoved, PropAdded);

                    Added?.Invoke(this, new PropStoreEventArgs(prop));
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
                    // запоминаем предыдущее состояние для событий
                    var oldProp = CalcActualItems()?.SingleOrDefault(x => x.Scheme == prop.Scheme);

                    TransferResource(resource, PropRemoved, PropAdded);

                    // Выброс событий
                    var currentProp = CalcActualItems()?.SingleOrDefault(x => x.Scheme == prop.Scheme);

                    if (currentProp != null)
                    {
                        Changed?.Invoke(this, new PropStoreEventArgs(oldProp));
                    }
                    else
                    {
                        Removed?.Invoke(this, new PropStoreEventArgs(oldProp));
                    }

                    break;

                case Equipment _:
                case Concept _:

                    TransferNoCount(prop, PropAdded, PropRemoved);

                    Removed?.Invoke(this, new PropStoreEventArgs(prop));

                    break;

                default:
                    throw new ArgumentException($"Предмет неизвестного типа {prop?.GetType()}.");
            }
        }

        public bool Has(IProp prop)
        {
            //TODO Неправильная реализация
            // Не учитывает списки добавленных и удалённых предметов.
            return PropStore.Has(prop);
        }
    }
}