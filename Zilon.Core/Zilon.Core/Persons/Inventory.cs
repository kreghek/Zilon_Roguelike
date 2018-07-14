using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Persons
{
    public class Inventory : IPropStore
    {
        private readonly HashSet<IProp> _items;

        public event EventHandler<InventoryEventArgs> Added;
        public event EventHandler<InventoryEventArgs> Removed;
        public event EventHandler<InventoryEventArgs> Changed;

        public Inventory()
        {
            _items = new HashSet<IProp>();
        }

        public IProp[] Items => _items.ToArray();

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

                case Recipe recipe:
                    AddRecipe(recipe);
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

                case Recipe recipe:
                    RemoveRecipe(recipe);
                    break;
            }
        }

        protected void DoChangedProp(IProp prop)
        {
            Changed?.Invoke(this, new InventoryEventArgs(new[] { prop }));
        }

        protected void DoRemovedProp(IProp prop)
        {
            Removed?.Invoke(this, new InventoryEventArgs(new[] { prop }));
        }

        protected void DoAddProp(IProp prop)
        {
            Added?.Invoke(this, new InventoryEventArgs(new[] { prop }));
        }

        private void RemoveEquipment(Equipment equipment)
        {
            _items.Remove(equipment);
            DoRemovedProp(equipment);
        }

        private void AddRecipe(Recipe recipe)
        {
            _items.Add(recipe);
            DoAddProp(recipe);
        }

        private void AddEquipment(Equipment equipment)
        {
            _items.Add(equipment);
            DoAddProp(equipment);
        }

        private void StackResource(Resource resource)
        {
            var currentResource = _items.OfType<Resource>()
                .SingleOrDefault(x => x.Scheme == resource.Scheme);

            if (currentResource == null)
            {
                _items.Add(resource);
                DoAddProp(resource);
            }
            else
            {
                currentResource.Count += resource.Count;
                DoChangedProp(currentResource);
            }
        }

        private void RemoveRecipe(Recipe recipe)
        {
            _items.Remove(recipe);
        }

        private void RemoveResource(Resource resource)
        {
            var currentResource = _items.OfType<Resource>()
                .SingleOrDefault(x => x.Scheme == resource.Scheme);

            if (currentResource == null)
            {
                throw new InvalidOperationException($"В инвентаре не найден ресурс со схемой {resource.Scheme}.");
            }

            if (currentResource.Count < resource.Count)
            {
                throw new InvalidOperationException($"Попытка удалить {resource.Count} ресурсов {resource.Scheme} больше чем есть в инвентаре.");
            }

            currentResource.Count -= resource.Count;
            if (currentResource.Count == 0)
            {
                _items.Remove(currentResource);
                DoRemovedProp(currentResource);
            }
            else
            {
                DoChangedProp(currentResource);
            }
        }
    }
}
