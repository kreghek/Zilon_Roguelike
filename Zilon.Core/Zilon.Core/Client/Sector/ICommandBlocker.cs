using System;

namespace Zilon.Core.Client.Sector
{
    /// <summary>
    /// The blocker of new command.
    /// </summary>
    /// <remarks>
    /// The player can't push new commands until the game has blockers.
    /// Blockers used for move or attack animations. So a user sees animation
    /// and gives next command after he've saw result of previous command.
    /// </remarks>
    public interface ICommandBlocker
    {
        void Release();
        event EventHandler? Released;
    }
}