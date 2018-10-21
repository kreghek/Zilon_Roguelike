namespace Zilon.Core.Schemes
{
    public interface IMonsterScheme: IScheme
    {
        IMonsterDefenseSubScheme Defense { get; }
        string[] DropTableSids { get; }
        int Hp { get; }
        ITacticalActStatsSubScheme PrimaryAct { get; }
    }
}