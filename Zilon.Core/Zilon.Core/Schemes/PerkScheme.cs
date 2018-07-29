namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема перка.
    /// </summary>
    public class PerkScheme: SchemeBase
    {
        public PerkConditionSubScheme[] BaseConditions { get; set; }
        public PerkConditionSubScheme[] VisibleConditions { get; set; }
        public PropSet[] Sources { get; set; }
        public PerkLevelSubScheme[] Levels { get; set; }

        public bool IsBuildIn { get; set; }

        public int Order { get; set; }
        public string IconSid { get; set; }
    }
}
