using System.Diagnostics.CodeAnalysis;
using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема условий для перка.
    /// </summary>
    /// <remarks>
    /// Используется для расчёта видимости и доступности перка для прокачки.
    /// </remarks>
    public class PerkConditionSubScheme : SubSchemeBase
    {
        /// <summary>
        /// Доступно для указанных классов.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string[] ClassesRequired { get; set; }

        /// <summary>
        /// Требует наличия родительского перка.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string ParentPerkSid { get; set; }

        /// <summary>
        /// Требует у родительского перка суммарный уровень владения не ниже указанного.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int MinParentPerkLevel { get; set; }

        /// <summary>
        /// Требует у родительского перка суммарный уровень владения не выше указанного.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int MaxParentPerkLevel { get; set; }

        /// <summary>
        /// Требует уровень персонажа не ниже указанного.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int LevelRequired { get; set; }

        /// <summary>
        /// Требует наличия предмета у персонажа.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public PropSet PropRequired { get; set; }
    }
}