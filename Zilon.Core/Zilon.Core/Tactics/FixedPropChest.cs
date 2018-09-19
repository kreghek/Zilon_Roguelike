using System;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public class FixedPropChest : IPropContainer
    {
        private bool _isOpened;

        public IMapNode Node { get; }

        public IPropStore Content { get; }
        public bool IsOpened
        {
            get => _isOpened;
            set
            {
                _isOpened = value;
                DoSetIsOpened();
            }
        }

        public int Id { get; set; }

        public bool IsMapBlock => true;

        private void DoSetIsOpened()
        {
            IsOpenChanged?.Invoke(this, EventArgs.Empty);
        }

        public FixedPropChest(IMapNode node, IProp[] props)
        {
            Node = node;
            Content = new Inventory();
            foreach (var prop in props)
            {
                Content.Add(prop);
            }
        }

        public event EventHandler IsOpenChanged;
    }
}
