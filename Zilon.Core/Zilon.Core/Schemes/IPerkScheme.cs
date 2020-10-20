using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    public interface IPerkScheme : IScheme
    {
        PerkConditionSubScheme[] BaseConditions { get; set; }
        string IconSid { get; set; }
        bool IsBuildIn { get; }
        JobSubScheme[] Jobs { get; set; }
        PerkLevelSubScheme[] Levels { get; set; }
        int Order { get; set; }
        PerkRuleSubScheme[] Rules { get; set; }
        PropSet[] Sources { get; set; }
        PerkConditionSubScheme[] VisibleConditions { get; set; }
    }
}