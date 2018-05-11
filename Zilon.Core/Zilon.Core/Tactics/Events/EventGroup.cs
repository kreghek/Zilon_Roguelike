namespace Zilon.Core.Tactics.Events
{
    using Newtonsoft.Json;

    public class EventGroup : CommandEventBase
    {
        public override string Id => "event-group";

        private ICommandEvent[] events;

        [JsonProperty("events")]
        public ICommandEvent[] Events => events;

        public EventGroup(string triggerName, TargetTriggerGroup[] targets, ICommandEvent[] events) : base(triggerName, targets)
        {
            this.events = events;
        }
    }
}
