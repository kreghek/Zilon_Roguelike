namespace Zilon.Core.Tactics.Behaviour
{
    public class HandOpenContainerMethod : IOpenContainerMethod
    {
        public IOpenContainerResult TryOpen(IPropContainer container)
        {
            return new SuccessOpenContainerResult(null);
        }
    }
}
