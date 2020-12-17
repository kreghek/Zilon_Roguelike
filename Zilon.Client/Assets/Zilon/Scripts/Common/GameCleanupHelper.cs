using Zilon.Core.Players;

namespace Assets.Zilon.Scripts.Common
{
    internal static class GameCleanupHelper
    {
        public static void ResetState(
            IPlayer player)
        {
            player.Reset();
        }
    }
}
