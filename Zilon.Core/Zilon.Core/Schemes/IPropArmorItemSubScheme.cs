using Zilon.Core.Common;
using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    ///     Элемент характеристик брони экипировки.
    /// </summary>
    public interface IPropArmorItemSubScheme
    {
        /// <summary>
        ///     Тип урона, поглощаемого бронёй.
        /// </summary>
        ImpactType Impact { get; }

        /// <summary>
        ///     Уровень поглощения урона бронёй.
        /// </summary>
        PersonRuleLevel AbsorbtionLevel { get; }

        /// <summary>
        ///     Ранг брони.
        /// </summary>
        int ArmorRank { get; }
    }
}