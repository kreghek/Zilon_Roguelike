using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс подсхемы для одного типа обороны монстра.
    /// </summary>
    public interface IMonsterDefenceItemSubScheme: ISubScheme
    {
        /// <summary>
        /// Тип обороны.
        /// </summary>
        DefenceType Type { get; }

        /// <summary>
        /// Уровень обороны.
        /// </summary>
        PersonRuleLevel Level { get; }
    }
}
