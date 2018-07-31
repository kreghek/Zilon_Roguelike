namespace Zilon.Core.Schemes
{
    public class MonsterScheme: SchemeBase
    {
        /// <summary>
        /// Действия, которые может использовать монстр.
        /// </summary>
        TacticalActStatsSubScheme PrimaryAct { get; set; }
    }
}
