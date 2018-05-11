namespace Zilon.Core.Tactics.Events
{
    using Newtonsoft.Json;

    public class TargetTriggerGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("triggers")]
        public string[] Triggers { get; set; }
    }
}
