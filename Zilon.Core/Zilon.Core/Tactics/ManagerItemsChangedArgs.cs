using System;

namespace Zilon.Core.Tactics
{
    public sealed class ManagerItemsChangedArgs<TItem> : EventArgs
    {
        public ManagerItemsChangedArgs(params TItem[] items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public TItem[] Items { get; }
    }
}
