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

    public sealed class StaticObjectGenerationContext : IStaticObjectGenerationContext
    {
        public StaticObjectGenerationContext(
            ISector sector,
            ISectorSubScheme scheme,
            IResourceDepositData resourceDepositData)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
            ResourceDepositData = resourceDepositData ?? throw new ArgumentNullException(nameof(resourceDepositData));
        }

        public ISector Sector { get; }

        public ISectorSubScheme Scheme { get; }

        public IResourceDepositData ResourceDepositData { get; }
    }
}