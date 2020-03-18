using Newtonsoft.Json;

using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема перка.
    /// </summary>
    public class PerkScheme : SchemeBase, IPerkScheme
    {
        public PerkRuleSubScheme[] Rules { get; set; }
        public JobSubScheme[] Jobs { get; set; }

        public PerkConditionSubScheme[] BaseConditions { get; set; }
        public PerkConditionSubScheme[] VisibleConditions { get; set; }
        public PropSet[] Sources { get; set; }
        public PerkLevelSubScheme[] Levels { get; set; }

        [JsonProperty]
        public bool IsBuildIn { get; private set; }

        public int Order { get; set; }
        public string IconSid { get; set; }
    }
}
