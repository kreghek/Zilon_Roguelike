using Newtonsoft.Json;
using Zilon.Logic.Math;

namespace Zilon.Logic.Tactics.Events
{
    public class ActorMovedEvent : CommandEventBase
    {
        private readonly int actorId;
        private readonly Vector2 start;
        private readonly Vector2 finish;

        public override string Id => "actor-moved";

        [JsonProperty("actorId")]
        public int ActorId => actorId;

        [JsonProperty("start")]
        public Vector2 Start => start;

        [JsonProperty("finish")]
        public Vector2 Finish => finish;

        public ActorMovedEvent(string triggerName, TargetTriggerGroup[] targetTriggers, int actorId, Vector2 start, Vector2 finish) :
            base(triggerName, targetTriggers)
        {
            this.actorId = actorId;
            this.start = start;
            this.finish = finish;
        }
    }
}
