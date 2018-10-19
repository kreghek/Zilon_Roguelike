using Zilon.Core.Components;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tests.Common.Schemes
{
    public sealed class TestPerkScheme : IPerkScheme
    {
        public PerkConditionSubScheme[] BaseConditions { get; set; }
        public string IconSid { get; set; }
        public bool IsBuildIn { get; set; }
        public JobSubScheme[] Jobs { get; set; }
        public PerkLevelSubScheme[] Levels { get; set; }
        public int Order { get; set; }
        public PerkRuleSubScheme[] Rules { get; set; }
        public PropSet[] Sources { get; set; }
        public PerkConditionSubScheme[] VisibleConditions { get; set; }
        public string Sid { get; set; }
        public bool Disabled { get; }
        public LocalizedStringSubScheme Name { get; }
        public LocalizedStringSubScheme Description { get; }
    }
}
