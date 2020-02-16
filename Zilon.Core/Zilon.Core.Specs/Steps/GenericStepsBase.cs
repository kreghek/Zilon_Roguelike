using Zilon.Core.Specs.Contexts;

namespace Zilon.Core.Specs.Steps
{
    public abstract class GenericStepsBase<TContext> where TContext : FeatureContextBase, new()
    {
        protected readonly TContext Context;

        protected GenericStepsBase(TContext context)
        {
            Context = context;
        }
    }
}
