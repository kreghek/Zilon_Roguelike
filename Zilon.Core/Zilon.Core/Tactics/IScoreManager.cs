using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Менеджер подсчёта очков.
    /// </summary>
    public interface IScoreManager
    {
        /// <summary>Базовые очки, набранные игроком.</summary>
        int BaseScores { get; }

        /// <summary>Фраги по схемам монстров, добытые игроком.</summary>
        IDictionary<IMonsterScheme, int> Frags { get; }

        /// <summary>Засчитать убийство монстра.</summary>
        /// <param name="monster"> Монстр, убитый игроком. </param>
        void CountMonsterDefeat(MonsterPerson monster);
    }
}