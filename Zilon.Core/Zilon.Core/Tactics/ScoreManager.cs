using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Реализация менеджера подсчёта очков.
    /// </summary>
    /// <seealso cref="Zilon.Core.Tactics.IScoreManager" />
    public class ScoreManager : IScoreManager
    {
        private const float TURN_INC = 0.1f;
        private const int PLACE_SCORES = 100;
        private const int MONSTER_DEFAULT_BASE_SCORE = 25;

        private float _turnCounter = 0;

        public ScoreManager()
        {
            Frags = new Dictionary<IMonsterScheme, int>();
            PlaceTypes = new Dictionary<ILocationScheme, int>();
            Places = new HashSet<GlobeRegionNode>();
        }

        /// <summary>Базовые очки, набранные игроком.</summary>
        public int BaseScores { get; private set; }

        /// <summary>Фраги по схемам монстров, добытые игроком.</summary>
        public IDictionary<IMonsterScheme, int> Frags { get; }

        /// <summary>Счётчик ходов по типам секторов.</summary>
        public IDictionary<ILocationScheme, int> PlaceTypes { get; }

        /// <summary>Шаги, прожитые персонажем.</summary>
        public int Turns { get; set; }

        /// <summary>Посещённые места.</summary>
        public ISet<GlobeRegionNode> Places { get; }

        /// <summary>Засчитать убийство монстра.</summary>
        /// <param name="monster">Монстр, убитый игроком.</param>
        public void CountMonsterDefeat(MonsterPerson monster)
        {
            var monsterScheme = monster.Scheme;

            var score = monsterScheme.BaseScore;
            if (score == 0)
            {
                score = MONSTER_DEFAULT_BASE_SCORE;
            }

            BaseScores += score;

            if (!Frags.ContainsKey(monsterScheme))
            {
                Frags.Add(monsterScheme, 0);
            }

            Frags[monsterScheme]++;
        }

        /// <summary>Засчитывает посещение места на глобальной карте.</summary>
        /// <param name="regionNode">Узел провинции, которая считается посещённым местом.</param>
        public void CountPlace(GlobeRegionNode regionNode)
        {
            if (!Places.Contains(regionNode))
            {
                Places.Add(regionNode);
                BaseScores += PLACE_SCORES;
            }
        }

        /// <summary>Засчитать один прожитый шаг.</summary>
        public void CountTurn(ILocationScheme sectorScheme)
        {
            _turnCounter += TURN_INC;
            if (_turnCounter >= 1)
            {
                _turnCounter -= 1;
                BaseScores++;
            }

            Turns++;

            if (!PlaceTypes.ContainsKey(sectorScheme))
            {
                PlaceTypes.Add(sectorScheme, 0);
            }

            PlaceTypes[sectorScheme]++;
        }
    }
}
