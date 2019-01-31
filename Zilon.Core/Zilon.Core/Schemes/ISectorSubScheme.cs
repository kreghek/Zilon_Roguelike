namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс схемы для создания сектора.
    /// </summary>
    public interface ISectorSubScheme: ISubScheme
    {
        /// <summary>
        /// Идентфиикаторы обычных монстров, встречаемых в секторе.
        /// </summary>
        string[] RegularMonsterSids { get; set; }

        /// <summary>
        /// Идентификаторы редких монстров, встречаемых в секторе.
        /// </summary>
        string[] RareMonsterSids { get; set; }

        /// <summary>
        /// Идентификаторы боссов, встречаемых в секторе.
        /// </summary>
        string[] ChampionMonsterSids { get; set; }
    }
}
