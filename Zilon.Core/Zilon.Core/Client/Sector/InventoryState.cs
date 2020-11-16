using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Client
{
    public class InventoryState : IInventoryState
    {
        private IPropItemViewModel _selectedProp;

        [ExcludeFromCodeCoverage]
        public IPropItemViewModel SelectedProp
        {
            get => _selectedProp;
            set
            {
                _selectedProp = value;
                SelectedPropChanged?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler SelectedPropChanged;
    }
}