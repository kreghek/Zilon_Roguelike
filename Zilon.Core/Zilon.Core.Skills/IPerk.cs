using Zilon.Core.Schemes;
using Zilon.Core.Skills;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Information about perk - archieved or potentially.
    /// </summary>
    public interface IPerk<TJobScheme> : IJobExecutable<TJobScheme> where TJobScheme : IMinimalJobSubScheme
    {
        /// <summary>
        /// Current perk level. Null current level means perk is not archieved.
        /// </summary>
        PerkLevel? CurrentLevel { get; set; }

        /// <summary>
        /// Perk scheme.
        /// </summary>
        IMinimalPerkScheme Scheme { get; }
    }

    public interface IMinimalPerkScheme
    {
        IPerkConditionSubScheme[]? BaseConditions { get; set; }
        string? IconSid { get; set; }
        bool IsBuildIn { get; }
        IMinimalJobSubScheme[]? Jobs { get; set; }
        PerkLevelSubScheme?[]? Levels { get; set; }
        int Order { get; set; }
        PerkRuleSubScheme?[]? Rules { get; set; }
        PropSet?[]? Sources { get; set; }
        PerkConditionSubScheme?[]? VisibleConditions { get; set; }
    }

    /// <summary>
    /// Схема условий для перка.
    /// </summary>
    /// <remarks>
    /// Используется для расчёта видимости и доступности перка для прокачки.
    /// </remarks>
    public interface IPerkConditionSubScheme
    {
        /// <summary>
        /// Требует у родительского перка суммарный уровень владения не выше указанного.
        /// </summary>
        int MaxParentPerkLevel { get; set; }

        /// <summary>
        /// Требует у родительского перка суммарный уровень владения не ниже указанного.
        /// </summary>
        int MinParentPerkLevel { get; set; }

        /// <summary>
        /// Требует наличия родительского перка.
        /// </summary>
        string? ParentPerkSid { get; set; }
    }
}