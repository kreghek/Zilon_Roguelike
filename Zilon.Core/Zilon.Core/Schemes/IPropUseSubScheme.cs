using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема предмета для хранения характеристик при применении предмета.
    /// </summary>
    public interface IPropUseSubScheme
    {
        /// <summary>
        /// Признак того, что при использовании будет уменьшен на единицу.
        /// </summary>
        bool Consumable { get; }

        /// <summary>
        /// Общие правила влияния.
        /// </summary>
        ConsumeCommonRule[] CommonRules { get; }

        //TODO Убрать, когда будут мысли, как можно задавать конкретные числа для правил
        int Value { get; }
    }
}