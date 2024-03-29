﻿using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Information about perk - achieved or potentially.
    /// </summary>
    public interface IPerk : IJobExecutable
    {
        /// <summary>
        /// Current perk level. Null current level means perk is not achieved.
        /// </summary>
        PerkLevel? CurrentLevel { get; set; }

        /// <summary>
        /// Perk scheme.
        /// </summary>
        IPerkScheme Scheme { get; }
    }
}