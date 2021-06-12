using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public class TestSectorTransitionSubScheme : ISectorTransitionSubScheme
    {
        public string SectorLevelSid { get; set; }
    }
}