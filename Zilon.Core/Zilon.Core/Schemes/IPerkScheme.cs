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

    /// <summary>
    /// Do not inherit from <see cref="IPerkScheme"/> because ISchemeService can collect all perks.
    /// </summary>
    public interface ISpawnPerkScheme : IScheme
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

        IPersonScheme PersonScheme { get; }
        ITacticalActScheme TacticalAct { get; }
    }
}