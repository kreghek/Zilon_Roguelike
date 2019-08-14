using System;

namespace Zilon.Core.Client
{
    public interface IInventoryState
    {
        IPropItemViewModel SelectedProp { get; set; }

        event EventHandler SelectedPropChanged;
    }
}