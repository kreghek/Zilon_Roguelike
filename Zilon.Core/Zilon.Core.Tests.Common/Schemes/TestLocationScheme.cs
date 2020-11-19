using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public sealed class TestLocationScheme : SchemeBase, ILocationScheme
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Эта структура используется для десериализации. Можно возвращать массив.")]
        public ISectorSubScheme[] SectorLevels { get; set; }
    }
}