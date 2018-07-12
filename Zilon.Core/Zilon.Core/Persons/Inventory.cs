using System;
using System.Collections.Generic;

namespace Zilon.Core.Persons
{
    public class Inventory
    {
        private readonly List<PropBase> _items;

        public Inventory()
        {
            _items = new List<PropBase>();
        }

        public PropBase[] Items => _items.ToArray();

        public void Add(PropBase prop)
        {
            throw new NotImplementedException();
        }

        public void Remove(PropBase prop)
        {
            throw new NotImplementedException();
        }
    }
}
