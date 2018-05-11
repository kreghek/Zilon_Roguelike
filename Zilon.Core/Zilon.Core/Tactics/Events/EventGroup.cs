namespace Zilon.Core.Tactics.Events
{
    using Newtonsoft.Json;

    public class EventGroup : CommandEventBase
    {
        public override string Id => "event-group";

        private ITacticEvent[] events;

        [JsonProperty("events")]
        public ITacticEvent[] Events => events;

        public EventGroup(string triggerName, TargetTriggerGroup[] targets, ITacticEvent[] events) : base(triggerName, targets)
        {
            this.events = events;
        }
    }
}
