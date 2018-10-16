using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Props;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public abstract class ChestBase : IPropContainer
    {
        [ExcludeFromCodeCoverage]
        protected ChestBase(IMapNode node, IPropStore content) : this(node, content, 0)
        {
        }

        [ExcludeFromCodeCoverage]
        protected ChestBase(IMapNode node, IPropStore content, int id)
        {
            Id = id;
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public int Id { get; }
        public IMapNode Node { get; }
        public IPropStore Content { get; }
        public bool IsOpened { get; private set; }
        public abstract bool IsMapBlock { get; }

        public event EventHandler Opened;

        public void Open()
        {
            IsOpened = true;
            DoSetIsOpened();
        }

        private void DoSetIsOpened()
        {
            Opened?.Invoke(this, EventArgs.Empty);
        }
    }
}
