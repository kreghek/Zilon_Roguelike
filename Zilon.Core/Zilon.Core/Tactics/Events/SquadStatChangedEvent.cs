namespace Zilon.Core.Tactics.Events
{
    using Newtonsoft.Json;

    public class SquadStatChangedEvent : CommandEventBase
    {
        private readonly int _squadId;
        private readonly int _mp;

        public override string Id => "squad-stat-changed";

        [JsonProperty("squadId")]
        public int SquadId => _squadId;

        [JsonProperty("mp")]
        public int Mp => _mp;

        public SquadStatChangedEvent(string triggerName, TargetTriggerGroup[] targetTriggers, int squadId, int mp) :
            base(triggerName, targetTriggers)
        {
            _squadId = squadId;
            _mp = mp;
        }
    }
}
