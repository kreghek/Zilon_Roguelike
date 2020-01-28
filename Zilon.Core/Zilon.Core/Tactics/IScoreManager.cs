using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Менеджер подсчёта очков.
    /// </summary>
    public interface IScoreManager
    {
        /// <summary>Базовые очки, набранные игроком.</summary>
        int BaseScores { get; }

        /// <summary>Шаги, прожитые персонажем.</summary>
        int Turns { get; }

        ScoreAchievements Achievements { get; }

        /// <summary>Фраги по схемам монстров, добытые игроком.</summary>
        IDictionary<IMonsterScheme, int> Frags { get; }

        /// <summary>Счётчик ходов по типам секторов.</summary>
        IDictionary<ILocationScheme, int> PlaceTypes { get; }

        /// <summary>
        /// Посещённые места.
        /// </summary>
        ISet<ProvinceNode> Places { get; }

        /// <summary>Засчитать убийство монстра.</summary>
        /// <param name="monster"> Монстр, убитый игроком. </param>
        void CountMonsterDefeat(MonsterPerson monster);

        /// <summary> Засчитать один прожитый шаг. </summary>
        void CountTurn(ILocationScheme locationScheme);

        /// <summary>
        /// Засчитывает посещение места на глобальной карте.
        /// </summary>
        /// <param name="regionNode">Узел провинции, которая считается посещённым местом.</param>
        void CountPlace(ProvinceNode regionNode);

        /// <summary>
        /// Засчитывает посещение победного узла.
        /// </summary>
        void CountHome();

        /// <summary> Обнуление текущих очков. </summary>
        void ResetScores();

        Scores Scores { get; set; }
    }
}