using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Tests.Common.Schemes
{
    using Core.Schemes;

    [ExcludeFromCodeCoverage]
    public class TestMonsterScheme : SchemeBase, IMonsterScheme
    {
        public ITacticalActStatsSubScheme PrimaryAct { get; set; }
        public int BaseScore { get; set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public IMonsterDefenseSubScheme Defense { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public string[] DropTableSids { get; }

        public int Hp { get; set; }

        /// <inheritdoc cref="IMonsterScheme" />
        public string[] Tags { get; set; }

        /// <inheritdoc />
        public float? MoveSpeedFactor { get; }
    }
}