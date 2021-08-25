﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Props
{
    /// <summary>
    /// Базовый класс для всех хранилищ предметов.
    /// </summary>
    public abstract class PropStoreBase : IPropStore
    {
        protected PropStoreBase()
        {
            Items = new HashSet<IProp>();
        }

        protected HashSet<IProp> Items { get; }

        private void AddConcept(Concept concept)
        {
            Items.Add(concept);
            DoAddProp(concept);
        }

        private void AddEquipment(Equipment equipment)
        {
            Items.Add(equipment);
            DoAddProp(equipment);
        }

        private void DoAddProp(IProp prop)
        {
            DoEventInner(Added, prop);
        }

        private void DoChangedProp(IProp prop)
        {
            DoEventInner(Changed, prop);
        }

        private void DoEventInner(EventHandler<PropStoreEventArgs>? @event,
            IProp? prop)
        {
            if (prop == null)
            {
                throw new ArgumentNullException(nameof(prop));
            }

            @event?.Invoke(this, new PropStoreEventArgs(prop));
        }

        private void DoRemovedProp(IProp prop)
        {
            DoEventInner(Removed, prop);
        }

        private bool HasConcept(Concept concept)
        {
            throw new NotImplementedException();
        }

        private bool HasEquipment(Equipment equipment)
        {
            return Items.Contains(equipment);
        }

        private bool HasResource(Resource resource)
        {
            return Items.OfType<Resource>().Any(x => x.Scheme.Sid == resource.Scheme.Sid && x.Count >= resource.Count);
        }

        private void RemoveConcept(Concept concept)
        {
            Items.Remove(concept);
            DoRemovedProp(concept);
        }

        private void RemoveEquipment(Equipment equipment)
        {
            Items.Remove(equipment);
            DoRemovedProp(equipment);
        }

        private void RemoveResource(Resource resource)
        {
            var currentResource = Items.OfType<Resource>()
                .SingleOrDefault(x => x.Scheme == resource.Scheme);

            if (currentResource == null)
            {
                throw new InvalidOperationException($"В инвентаре не найден ресурс со схемой {resource.Scheme}.");
            }

            if (currentResource.Count < resource.Count)
            {
                throw new InvalidOperationException(
                    $"Попытка удалить {resource.Count} ресурсов {resource.Scheme} больше чем есть в инвентаре.");
            }

            if (currentResource.Count == resource.Count)
            {
                Items.Remove(currentResource);
                DoRemovedProp(currentResource);
            }
            else
            {
                currentResource.Count -= resource.Count;
                DoChangedProp(currentResource);
            }
        }

        private void StackResource(Resource resource)
        {
            var currentResource = Items.OfType<Resource>()
                .SingleOrDefault(x => x.Scheme == resource.Scheme);

            if (currentResource == null)
            {
                Items.Add(resource);
                DoAddProp(resource);
            }
            else
            {
                currentResource.Count += resource.Count;
                DoChangedProp(currentResource);
            }
        }

        public event EventHandler<PropStoreEventArgs>? Added;
        public event EventHandler<PropStoreEventArgs>? Removed;
        public event EventHandler<PropStoreEventArgs>? Changed;

        public virtual IProp[] CalcActualItems()
        {
            return Items.ToArray();
        }

        public void Add(IProp prop)
        {
            switch (prop)
            {
                case Equipment equipment:
                    AddEquipment(equipment);
                    break;

                case Resource resource:
                    StackResource(resource);
                    break;

                case Concept concept:
                    AddConcept(concept);
                    break;
            }
        }

        public void Remove(IProp prop)
        {
            switch (prop)
            {
                case Equipment equipment:
                    RemoveEquipment(equipment);
                    break;

                case Resource resource:
                    RemoveResource(resource);
                    break;

                case Concept concept:
                    RemoveConcept(concept);
                    break;
            }
        }

        public bool Has(IProp prop)
        {
            return prop switch
            {
                Equipment equipment => HasEquipment(equipment),
                Resource resource => HasResource(resource),
                Concept concept => HasConcept(concept),
                _ => throw new InvalidOperationException()
            };
        }
    }
}