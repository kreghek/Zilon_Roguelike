using Assets.Zilon.Scripts.Services;

using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Common
{
    internal static class GameCleanupHelper
    {
        public static void ResetState(
            IPlayer player,
            GlobeStorage globeStorage)
        {
            player.Reset();
            globeStorage.Reset();
        }
    }
}
