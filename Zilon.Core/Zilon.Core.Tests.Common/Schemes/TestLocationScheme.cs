using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public sealed class TestLocationScheme : SchemeBase, ILocationScheme
    {
        public ISectorSubScheme[] SectorLevels { get; set; }
    }
}
