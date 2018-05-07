using Newtonsoft.Json;
using Zilon.Logic.Math;

namespace Zilon.Logic.Tactics.Events
{
    public class SquadMovedEvent : CommandEventBase
    {
        private readonly int squadId;
        private readonly Vector2 start;
        private readonly Vector2 finish;

        public override string Id => "squad-moved";

        [JsonProperty("actorId")]
        public int SquadId => squadId;

        [JsonProperty("start")]
        public Vector2 Start => start;

        [JsonProperty("finish")]
        public Vector2 Finish => finish;

        public SquadMovedEvent(string triggerName, TargetTriggerGroup[] targetTriggers, int squadId, Vector2 start, Vector2 finish) :
            base(triggerName, targetTriggers)
        {
            this.squadId = squadId;
            this.start = start;
            this.finish = finish;
        }
    }
}
