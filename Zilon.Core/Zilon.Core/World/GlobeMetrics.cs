﻿using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.World
{
    /// <summary>
    /// Common metrics and restrictions of a globe. Looks like The Globe Laws.
    /// </summary>
    /// <remarks>
    /// This class only for constant values. Do not place calculation methods here.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    public static class GlobeMetrics
    {
        /// <summary>
        /// Duration of idle. Depends on <see cref="OneIterationLength" />.
        /// </summary>
        public static int IdleDuration => OneIterationLength;

        /// <summary>
        /// Max value for any monster's move cost. Actually, lowest possible monster move speed in game.
        /// </summary>
        public static int MaxMonsterMoveCost => OneIterationLength * 100;

        /// <summary>
        /// Min value for any monster's move cost. Actually, higher possible monster move speed in game.
        /// </summary>
        public static int MinMonsterMoveCost => 1;

        /// <summary>
        /// Count of globe update to execute actor tasks to next iteration.
        /// Iteration of the globe is:
        /// - Survival updates.
        /// - Events updates.
        /// - Desease updates.
        /// </summary>
        public static int OneIterationLength => 10;

        /// <summary>
        /// Transition limit from pool.
        /// The limit is required to prevent hanging.
        /// </summary>
        public static int TransitionPerGlobeIteration => 10;
    }
}