using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Client
{
    public class InventoryState : IInventoryState
    {
        [ExcludeFromCodeCoverage]
        public IPropItemViewModel SelectedProp { get; set; }
    }
}
