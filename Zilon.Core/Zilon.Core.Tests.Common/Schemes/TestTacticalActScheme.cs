﻿using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class TestTacticalActScheme : SchemeBase, ITacticalActScheme
    {
        public ITacticalActConstrainsSubScheme Constrains { get; set; }
        public ITacticalActStatsSubScheme Stats { get; set; }
        public string IsMimicFor { get; }
    }
}