using Zilon.Core.Client;

namespace Assets.Zilon.Scripts.Services
{
    public class InventoryState: IInventoryState
    {
        public IPropItemViewModel SelectedProp { get; set; }
    }
}