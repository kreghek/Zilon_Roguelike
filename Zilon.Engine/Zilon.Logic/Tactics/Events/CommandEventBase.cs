using Newtonsoft.Json;

namespace Zilon.Logic.Tactics.Events
{
    public abstract class CommandEventBase : ICommandEvent
    {
        protected string triggerName;
        protected TargetTriggerGroup[] targets;

        [JsonProperty("id")]
        public abstract string Id { get; }

        [JsonProperty("triggerName")]
        public string TriggerName => triggerName;

        [JsonProperty("targets")]
        public TargetTriggerGroup[] Targets => targets;

        public CommandEventBase(string triggerName, TargetTriggerGroup[] targets)
        {
            this.triggerName = triggerName;
            this.targets = targets;
        }
    }
}
