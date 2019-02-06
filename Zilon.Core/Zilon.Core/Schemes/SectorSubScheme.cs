namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Реализация схемы сектора.
    /// </summary>
    public sealed class SectorSubScheme : SubSchemeBase, ISectorSubScheme
    {
        /// <summary>
        /// Идентфиикаторы обычных монстров, встречаемых в секторе.
        /// </summary>
        public string[] RegularMonsterSids { get; set; }

        /// <summary>
        /// Идентификаторы редких монстров, встречаемых в секторе.
        /// </summary>
        public string[] RareMonsterSids { get; set; }

        /// <summary>
        /// Идентификаторы боссов, встречаемых в секторе.
        /// </summary>
        public string[] ChampionMonsterSids { get; set; }
    }
}
