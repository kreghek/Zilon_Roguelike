using Zilon.Core.Props;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Инвентарь персонажа.
    /// </summary>
    public sealed class InventoryModule : PropStoreBase, IInventoryModule
    {
        public InventoryModule()
        {
            IsActive = true;
        }

        public string Key => nameof(IInventoryModule);

        public bool IsActive { get; set; }
    }
}