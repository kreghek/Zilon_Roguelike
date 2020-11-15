using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.Scoring
{
    /// <summary>
    /// Менеджер подсчёта очков.
    /// </summary>
    public interface IScoreManager
    {
        ScoreAchievements Achievements { get; }

        /// <summary>Базовые очки, набранные игроком.</summary>
        int BaseScores { get; }

        /// <summary>Фраги по схемам монстров, добытые игроком.</summary>
        IDictionary<IMonsterScheme, int> Frags { get; }

        /// <summary>Счётчик ходов по типам секторов.</summary>
        IDictionary<ILocationScheme, int> PlaceTypes { get; }

        Scores Scores { get; set; }

        /// <summary>Шаги, прожитые персонажем.</summary>
        int Turns { get; }

        /// <summary>
        /// Засчитывает посещение победного узла.
        /// </summary>
        void CountHome();

        /// <summary>Засчитать убийство монстра.</summary>
        /// <param name="monster"> Монстр, убитый игроком. </param>
        void CountMonsterDefeat(MonsterPerson monster);

        /// <summary> Засчитать один прожитый шаг. </summary>
        void CountTurn(ILocationScheme locationScheme);

        /// <summary> Обнуление текущих очков. </summary>
        void ResetScores();
    }
}