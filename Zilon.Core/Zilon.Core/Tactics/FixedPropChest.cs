using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация сундука с фиксированным лутом.
    /// </summary>
    public class FixedPropChest : IPropContainer
    {
        private bool _isOpened;

        [ExcludeFromCodeCoverage]
        public IMapNode Node { get; }

        [ExcludeFromCodeCoverage]
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

        [ExcludeFromCodeCoverage]
        public int Id { get; set; }

        [ExcludeFromCodeCoverage]
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
