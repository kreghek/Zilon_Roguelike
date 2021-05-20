﻿using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public sealed class TestPerkScheme : SchemeBase, IPerkScheme
    {
        public string SystemDescription { get; set; }

        public override string ToString()
        {
            return SystemDescription;
        }

        public PerkConditionSubScheme[] BaseConditions { get; set; }
        public string IconSid { get; set; }
        public bool IsBuildIn { get; set; }
        public JobSubScheme[] Jobs { get; set; }
        public PerkLevelSubScheme[] Levels { get; set; }
        public int Order { get; set; }
        public PerkRuleSubScheme[] Rules { get; set; }
        public PropSet[] Sources { get; set; }
        public PerkConditionSubScheme[] VisibleConditions { get; set; }
    }
}