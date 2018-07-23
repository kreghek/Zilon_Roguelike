using Zilon.Core.Client;

namespace Assets.Zilon.Scripts.Services
{
    public interface IInventoryState
    {
        IPropItemViewModel SelectedProp { get; set; }
    }
}