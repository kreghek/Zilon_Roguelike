using Zilon.Core.Players;
using Zilon.Core.Scoring;

namespace Assets.Zilon.Scripts.Models.TitleScene
{
    //TODO Статический класс лучше заменить на отдельную службу со своими зависимостями.
    [System.Obsolete("Требуется заменить отдельной службой")]
    public static class GameProgressHelper
    {
        public static void ResetGameState(
            IScoreManager scoreManager,
            HumanPlayer humanPlayer)
        {
            humanPlayer.Reset();
            scoreManager.ResetScores();
        }
    }
}
