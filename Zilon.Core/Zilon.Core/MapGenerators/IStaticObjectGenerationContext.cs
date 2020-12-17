using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    public interface IStaticObjectGenerationContext
    {
        IResourceDepositData ResourceDepositData { get; }
        ISectorSubScheme Scheme { get; }
        ISector Sector { get; }
    }
}