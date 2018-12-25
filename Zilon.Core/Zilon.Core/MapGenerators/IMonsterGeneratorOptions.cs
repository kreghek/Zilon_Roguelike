using Zilon.Core.Players;

namespace Zilon.Core.MapGenerators
{
    public interface IMonsterGeneratorOptions
    {
        string[] RegularMonsterSids { get; set; }
        string[] RareMonsterSids { get; set; }
        string[] ChampionMonsterSids { get; set; }
        IBotPlayer BotPlayer { get; set; }
    }
}
