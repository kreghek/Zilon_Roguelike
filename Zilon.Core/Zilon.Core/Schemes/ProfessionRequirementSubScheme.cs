﻿namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Требуемые профессиональные компетенции.
    /// </summary>
    public sealed class ProfessionRequirementSubScheme : SubSchemeBase
    {
        /// <summary>
        /// Максимальный уровень владения.
        /// </summary>
        public int MaxLevel { get; set; }

        /// <summary>
        /// Минимальный уровень владения.
        /// </summary>
        public int MinLevel { get; set; }

        /// <summary>
        /// Тип компетенции.
        /// </summary>
        public ProfessionType Profession { get; set; }
    }
}