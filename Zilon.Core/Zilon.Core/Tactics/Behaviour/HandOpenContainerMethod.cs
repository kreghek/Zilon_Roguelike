namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Метод вскрытия контейнеров - руками, без использования чего-либо.
    /// </summary>
    public class HandOpenContainerMethod : IOpenContainerMethod
    {
        public IOpenContainerResult TryOpen(IPropContainer container)
        {
            container.Open();
            return new SuccessOpenContainerResult();
        }
    }
}
