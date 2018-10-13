using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс подсхемы для одного типа обороны монстра.
    /// </summary>
    public interface IMonsterDefenceItemSubScheme: ISchemeSubScheme
    {
        /// <summary>
        /// Тип обороны.
        /// </summary>
        DefenceType Defence { get; }

        /// <summary>
        /// Уровень обороны.
        /// </summary>
        PersonRuleLevel Level { get; }
    }
}
