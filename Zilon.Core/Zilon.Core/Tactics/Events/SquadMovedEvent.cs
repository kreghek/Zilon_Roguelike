namespace Zilon.Core.Tactics.Events
{
    using Newtonsoft.Json;

    public class SquadMovedEvent : CommandEventBase
    {
        private readonly int squadId;
        private readonly int startNodeId;
        private readonly int finishNodeId;

        public override string Id => "squad-moved";

        [JsonProperty("actorId")]
        public int SquadId => squadId;

        [JsonProperty("start")]
        public int StartNodeId => startNodeId;

        [JsonProperty("finish")]
        public int FinishNodeId => finishNodeId;

        public SquadMovedEvent(string triggerName, TargetTriggerGroup[] targetTriggers, int squadId, int startNodeId, int finishNodeId) :
            base(triggerName, targetTriggers)
        {
            this.squadId = squadId;
            this.startNodeId = startNodeId;
            this.finishNodeId = finishNodeId;
        }
    }
}
