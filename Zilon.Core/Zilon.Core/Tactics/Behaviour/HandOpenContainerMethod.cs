namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Метод вскрытия контейнеров - руками, без испоьзования чего-либо.
    /// </summary>
    public class HandOpenContainerMethod : IOpenContainerMethod
    {
        public IOpenContainerResult TryOpen(IPropContainer container)
        {
            var props = container.Inventory.Items;
            return new SuccessOpenContainerResult(props);
        }
    }
}
