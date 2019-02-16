using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация менеджера подсчёта очков.
    /// </summary>
    /// <seealso cref="Zilon.Core.Tactics.IScoreManager" />
    public class ScoreManager : IScoreManager
    {
        public ScoreManager()
        {
            Frags = new Dictionary<IMonsterScheme, int>();
        }

        /// <summary>Базовые очки, набранные игроком.</summary>
        public int BaseScores { get; private set; }

        /// <summary>Фраги по схемам монстров, добытые игроком.</summary>
        public IDictionary<IMonsterScheme, int> Frags { get; }

        /// <summary>Засчитать убийство монстра.</summary>
        /// <param name="monster">Монстр, убитый игроком.</param>
        public void CountMonsterDefeat(MonsterPerson monster)
        {
            var monsterScheme = monster.Scheme;

            BaseScores += monsterScheme.BaseScore;

            if (!Frags.ContainsKey(monsterScheme))
            {
                Frags.Add(monsterScheme, 0);
            }

            Frags[monsterScheme]++;
        }
    }
}
