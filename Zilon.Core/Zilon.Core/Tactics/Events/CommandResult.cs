namespace Zilon.Core.Tactics.Events
{
    using Newtonsoft.Json;

    public struct CommandResult
    {
        [JsonProperty("type")]
        public CommandResultType Type { get; set; }

        [JsonProperty("events")]
        public ICommandEvent[] Events { get; set; }

        [JsonProperty("errors")]
        public string[] Errors { get; set; }
    }
}
