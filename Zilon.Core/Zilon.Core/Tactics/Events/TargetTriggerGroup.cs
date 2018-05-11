using Newtonsoft.Json;

namespace Zilon.Logic.Tactics.Events
{
    public class TargetTriggerGroup
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("triggers")]
        public string[] Triggers { get; set; }
    }
}
