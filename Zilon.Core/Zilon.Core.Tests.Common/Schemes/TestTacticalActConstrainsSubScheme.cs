using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [ExcludeFromCodeCoverage]
    public class TestTacticalActConstrainsSubScheme : ITacticalActConstrainsSubScheme
    {
        /// <inheritdoc />
        public string PropResourceType { get; set; }

        /// <inheritdoc />
        public int? PropResourceCount { get; set; }

        /// <inheritdoc />
        public int? Cooldown { get; }

        /// <inheritdoc />
        public int? EnergyCost { get; }
    }
}