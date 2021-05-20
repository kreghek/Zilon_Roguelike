﻿namespace Zilon.Core.Tests.Common.Schemes
{
    using Core.Schemes;

    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TestMonsterScheme : SchemeBase, IMonsterScheme
    {
        public int BaseScore { get; set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public IMonsterDefenseSubScheme Defense { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public string[] DropTableSids { get; }

        public int Hp { get; set; }

        public ITacticalActStatsSubScheme PrimaryAct { get; set; }

        /// <inheritdoc cref="IMonsterScheme" />
        public string[] Tags { get; set; }

        /// <inheritdoc />
        public float? MoveSpeedFactor { get; }
    }
}