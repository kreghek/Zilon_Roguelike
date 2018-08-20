using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема предмета для хранения характеристик при применении предмета.
    /// </summary>
    public class PropUseSubScheme: SubSchemeBase
    {
        /// <summary>
        /// Признак того, что при использовании будет уменьшен на единицу.
        /// </summary>
        public bool Consumable { get; set; }

        /// <summary>
        /// Общие правила влияния.
        /// </summary>
        public ConsumeCommonRule[] CommonRules { get; set; }

        //TODO Убрать, когда будут мысли, как можно задавать конкретные числа для правил
        public int Value { get; set; }
    }
}
