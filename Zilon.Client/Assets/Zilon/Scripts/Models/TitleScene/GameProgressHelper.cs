using Zilon.Core.Players;
using Zilon.Core.Scoring;
using Zilon.Core.World;

namespace Assets.Zilon.Scripts.Models.TitleScene
{
    public static class GameProgressHelper
    {
        public static void ResetGameState(
            IGlobeManager globeManager,
            IScoreManager scoreManager,
            HumanPlayer humanPlayer)
        {
            globeManager.ResetGlobeState();
            humanPlayer.GlobeNode = null;
            humanPlayer.MainPerson = null;
            humanPlayer.SectorSid = null;
            humanPlayer.Terrain = null;
            scoreManager.ResetScores();
        }
    }
}
