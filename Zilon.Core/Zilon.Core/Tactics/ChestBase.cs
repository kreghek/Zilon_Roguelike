using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Graphs;
using Zilon.Core.Props;

namespace Zilon.Core.Tactics
{
    public abstract class ChestBase : IPropContainer
    {
        [ExcludeFromCodeCoverage]
        protected ChestBase(IGraphNode node, IPropStore content) : this(node, content, default(int))
        {
        }

        [ExcludeFromCodeCoverage]
        protected ChestBase(IGraphNode node, IPropStore content, int id)
        {
            Id = id;
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Content = content ?? throw new ArgumentNullException(nameof(content));

            Content.Added += Content_Added;
            Content.Removed += Content_Removed;
        }

        private void Content_Removed(object sender, PropStoreEventArgs e)
        {
            ItemsRemoved?.Invoke(this, e);
        }

        private void Content_Added(object sender, PropStoreEventArgs e)
        {
            ItemsAdded?.Invoke(this, e);
        }

        public int Id { get; }
        public IGraphNode Node { get; }
        public IPropStore Content { get; }
        public bool IsOpened { get; private set; }
        public abstract bool IsMapBlock { get; }
        public PropContainerPurpose Purpose { get; set; }

        public event EventHandler Opened;
        public event EventHandler<PropStoreEventArgs> ItemsAdded;
        public event EventHandler<PropStoreEventArgs> ItemsRemoved;

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
