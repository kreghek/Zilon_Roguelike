using System;

namespace Zilon.Core.Tactics
{
    public sealed class ManagerItemsChangedEventArgs<TItem> : EventArgs
    {
        public ManagerItemsChangedEventArgs(params TItem[] items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public TItem[] Items { get; }
    }
}
