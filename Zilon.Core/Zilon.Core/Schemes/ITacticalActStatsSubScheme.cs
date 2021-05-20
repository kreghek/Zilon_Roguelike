﻿using Zilon.Core.Common;
using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    public interface ITacticalActStatsSubScheme : ISubScheme
    {
        /// <summary>
        /// Эффект, который оказывает действие.
        /// </summary>
        TacticalActEffectType Effect { get; }

        /// <summary>
        /// Эффективность действия.
        /// </summary>
        Roll? Efficient { get; }

        /// <summary>
        /// Количество ударов при совершении действия.
        /// </summary>
        int HitCount { get; }

        /// <summary>
        /// Является ли действие рукопашным.
        /// </summary>
        /// <remarks>
        /// Рукопашные действия переводят актёра в режим рукопашного боя.
        /// Во время рукопашного режима можно использовать только рукопашные действия.
        /// </remarks>
        bool IsMelee { get; }

        /// <summary>
        /// Характеристики атакующей способности действия.
        /// </summary>
        ITacticalActOffenceSubScheme? Offence { get; }

        /// <summary>
        /// Дистанция, в котором возможно использования действия.
        /// </summary>
        Range<int>? Range { get; }

        /// <summary>
        /// Доступные цели действия.
        /// </summary>
        TacticalActTargets Targets { get; }
    }
}