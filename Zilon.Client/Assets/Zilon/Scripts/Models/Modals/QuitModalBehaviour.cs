namespace Assets.Zilon.Scripts.Models.Modals
{
    /// <summary>
    /// Behaviour of modal. Specify command and vizualization of modal.
    /// </summary>
    public enum QuitModalBehaviour
    {
        /// <summary>
        /// Ends game session. Go to title menu.
        /// </summary>
        QuitToTitleMenu = 1,

        /// <summary>
        /// Closes game.
        /// </summary>
        QuitGame = 2
    }
}
