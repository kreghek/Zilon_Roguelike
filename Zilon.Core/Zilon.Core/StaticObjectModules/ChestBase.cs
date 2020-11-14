using Zilon.Core.Props;
using Zilon.Core.Tactics;

namespace Zilon.Core.StaticObjectModules
{
    public abstract class ChestBase : IPropContainer
    {
        [ExcludeFromCodeCoverage]
        protected ChestBase(IPropStore content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));

            Content.Added += Content_Added;
            Content.Removed += Content_Removed;

            IsActive = true;
        }

        /// <inheritdoc/>
        public IPropStore Content { get; }

        /// <inheritdoc/>
        public bool IsOpened { get; private set; }

        /// <inheritdoc/>
        public PropContainerPurpose Purpose { get; set; }

        /// <inheritdoc/>
        public abstract bool IsMapBlock { get; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public string Key => nameof(IPropContainer);

        /// <inheritdoc/>
        public event EventHandler Opened;

        /// <inheritdoc/>
        public event EventHandler<PropStoreEventArgs> ItemsAdded;

        /// <inheritdoc/>
        public event EventHandler<PropStoreEventArgs> ItemsRemoved;

        /// <inheritdoc/>
        public void Open()
        {
            IsOpened = true;
            DoSetIsOpened();
        }

        /// <inheritdoc/>
        private void DoSetIsOpened()
        {
            Opened?.Invoke(this, EventArgs.Empty);
        }

        private void Content_Removed(object sender, PropStoreEventArgs e)
        {
            ItemsRemoved?.Invoke(this, e);
        }

        private void Content_Added(object sender, PropStoreEventArgs e)
        {
            ItemsAdded?.Invoke(this, e);
        }
    }
}