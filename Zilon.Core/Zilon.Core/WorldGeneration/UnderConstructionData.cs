using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Информация о конструировании структуры.
    /// </summary>
    public sealed class UnderConstructionData
    {
        /// <summary>
        /// Признак того, что структура ещё создаётся.
        /// Пока структура создаётся, на её содержание уже расходуются деньги. Но пользоваться её ещё нельзя.
        /// </summary>
        public bool UnderConstruction { get; set; }

        /// <summary>
        /// Планируемый объём работ для завершения конструирования структуры.
        /// По факту, сколько единиц производства нужно потратить, прежде чем
        /// структура будет готово к испольованию.
        /// </summary>
        public int PlanScopeOfWork { get; set; }

        /// <summary>
        /// Фактически выполненый объём работ. Когда будет равно плановому объёму работ,
        /// конструирование завершится и структура будет готова к вводу в эксплуатацию.
        /// </summary>
        public float FactScopeOfWork { get; set; }

        /// <summary>
        /// Ресурсы, необходимые для начала строительства структуры.
        /// </summary>
        public Dictionary<LocalityResource, int> StartConstructionResources { get; set; }
    }
}
