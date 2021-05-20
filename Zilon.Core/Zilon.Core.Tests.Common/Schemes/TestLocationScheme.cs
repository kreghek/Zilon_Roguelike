using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public sealed class TestLocationScheme : SchemeBase, ILocationScheme
    {
        [SuppressMessage("Performance", "CA1819:Properties should not return arrays",
            Justification = "Эта структура используется для десериализации. Можно возвращать массив.")]
        public ISectorSubScheme[] SectorLevels { get; set; }
    }
}