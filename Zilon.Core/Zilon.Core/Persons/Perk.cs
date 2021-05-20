﻿using System;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Текущее состояние перка персонажа.
    /// </summary>
    public class Perk : IPerk
    {
        public Perk(IPerkScheme scheme)
        {
            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
        }

        /// <summary>
        /// Достигнутые уровени перка.
        /// </summary>
        public PerkLevelSubScheme[]? ArchievedLevelSchemes { get; set; }

        /// <summary>
        /// Признак того, что целевой уровень перка проплачен.
        /// </summary>
        public bool IsLevelPaid { get; internal set; }

        /// <summary>
        /// Схема уровеня перка, которая прокачивается.
        /// </summary>
        public PerkLevelSubScheme? TargetLevelScheme { get; set; }

        /// <summary>
        /// Схема перка.
        /// </summary>
        public IPerkScheme Scheme { get; set; }

        /// <summary>
        /// Аккумулированные работы по перку.
        /// Засчитываются в зависимости от области действия работ перка.
        /// </summary>
        public IJob[]? CurrentJobs { get; set; }

        /// <summary>
        /// Текущий уровень перка.
        /// </summary>
        public PerkLevel? CurrentLevel { get; set; }
    }
}