namespace Zilon.TextClient
{
    /// <summary>
    /// Identifier of a game screen.
    /// A game screen is a game state.
    /// </summary>
    internal enum GameScreen
    {
        /// <summary>
        /// Undefined screen may be if error in the code.
        /// </summary>
        Undefinded,

        /// <summary>
        /// Globe generation and selection.
        /// </summary>
        GlobeSelection,

        /// <summary>
        /// Main game screen.
        /// </summary>
        Main,

        /// <summary>
        /// Game screen with scores and achievements.
        /// </summary>
        Scores
    }
}