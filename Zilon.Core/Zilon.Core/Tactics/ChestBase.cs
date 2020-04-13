using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Props;

namespace Zilon.Core.Tactics
{
    public abstract class ChestBase : IPropContainer
    {
        [ExcludeFromCodeCoverage]
        protected ChestBase(IPropStore content)
        {
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

        public IPropStore Content { get; }
        public bool IsOpened { get; private set; }
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
