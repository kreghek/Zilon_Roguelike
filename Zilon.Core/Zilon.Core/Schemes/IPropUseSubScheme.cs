namespace Zilon.Core.Schemes
{
    /// <summary>
    ///     Подсхема предмета для хранения характеристик при применении предмета.
    /// </summary>
    public interface IPropUseSubScheme
    {
        /// <summary>
        ///     Признак того, что при использовании будет уменьшен на единицу.
        /// </summary>
        bool Consumable { get; }

        /// <summary>
        ///     Общие правила влияния.
        /// </summary>
        ConsumeCommonRule[] CommonRules { get; }
    }
}