using Zilon.Core.Players;

namespace Zilon.Core.MapGenerators
{
    public sealed class MonsterGeneratorOptions : IMonsterGeneratorOptions
    {
        public string[] RegularMonsterSids { get; set; }
        public string[] RareMonsterSids { get; set; }
        public string[] ChampionMonsterSids { get; set; }
        public IBotPlayer BotPlayer { get; set; }
    }
}
