namespace Zilon.Core.World
{
    public static class GlobeMetrics
    {
        /// <summary>
        /// Duration of idle. Depends on <see cref="OneIterationLength" />.
        /// </summary>
        public static int IdleDuration => OneIterationLength / 3;

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

        /// <summary>
        /// Min value for any monster's move cost. Actually, higher possible monster move speed in game.
        /// </summary>
        public static int MinMonsterMoveCost => 1;

        /// <summary>
        /// Max value for any monster's move cost. Actually, lowest possible monster move speed in game.
        /// </summary>
        public static int MaxMonsterMoveCost => OneIterationLength * 100;
    }
}