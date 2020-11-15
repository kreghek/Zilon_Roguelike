namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема предмета для хранения характеристик при применении предмета.
    /// </summary>
    public class PropUseSubScheme : SubSchemeBase, IPropUseSubScheme
    {
        /// <summary>
        /// Признак того, что при использовании будет уменьшен на единицу.
        /// </summary>
        [JsonProperty]
        public bool Consumable { get; private set; }

        /// <summary>
        /// Общие правила влияния.
        /// </summary>
        [JsonProperty]
        public ConsumeCommonRule[] CommonRules { get; private set; }
    }
}