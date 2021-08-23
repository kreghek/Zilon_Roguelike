using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Tests.Common.Schemes
{
    using Core.Schemes;

    [ExcludeFromCodeCoverage]
    public class TestMonsterScheme : SchemeBase, IMonsterScheme
    {
        /// <inheritdoc />
        public ITacticalActStatsSubScheme PrimaryAct { get; set; }

        /// <inheritdoc />
        public int BaseScore { get; set; }

        /// <inheritdoc />
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public IMonsterDefenseSubScheme Defense { get; }

        /// <inheritdoc />
        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public string[] DropTableSids { get; }

        /// <inheritdoc />
        public int Hp { get; set; }

        /// <inheritdoc cref="IMonsterScheme" />
        public string[] Tags { get; set; }

        /// <inheritdoc />
        public float? MoveSpeedFactor { get; }

        /// <inheritdoc />
        public ITacticalActStatsSubScheme[] CombatActs { get; }
    }
}