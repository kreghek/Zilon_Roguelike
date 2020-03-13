using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.Scoring
{
    /// <summary>
    /// Реализация менеджера подсчёта очков.
    /// </summary>
    /// <seealso cref="IScoreManager" />
    public class ScoreManager : IScoreManager
    {
        private const float TURN_INC = 0.1f;
        private const int PLACE_SCORES = 100;
        private const int MONSTER_DEFAULT_BASE_SCORE = 25;

        public ScoreManager()
        {
            Scores = new Scores();
        }

        /// <inheritdoc/>
        public int BaseScores { get => Scores.BaseScores; private set => Scores.BaseScores = value; }

        /// <inheritdoc/>
        public IDictionary<IMonsterScheme, int> Frags { get => Scores.Frags; }

        /// <inheritdoc/>
        public IDictionary<ILocationScheme, int> PlaceTypes { get => Scores.PlaceTypes; }

        /// <inheritdoc/>
        public int Turns { get => Scores.Turns; set => Scores.Turns = value; }

        /// <inheritdoc/>
        public ISet<GlobeRegionNode> Places { get => Scores.Places; }

        /// <inheritdoc/>
        public ScoreAchievements Achievements { get => Scores.Achievements; private set => Scores.Achievements = value; }

        public void CountHome()
        {
            Achievements |= ScoreAchievements.HomeFound;
            BaseScores *= 2;
        }

        /// <summary>Засчитать убийство монстра.</summary>
        /// <param name="monster">Монстр, убитый игроком.</param>
        public void CountMonsterDefeat(MonsterPerson monster)
        {
            if (monster is null)
            {
                throw new System.ArgumentNullException(nameof(monster));
            }

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
            Scores.TurnCounter += TURN_INC;
            if (Scores.TurnCounter >= 1)
            {
                Scores.TurnCounter -= 1;
                BaseScores++;
            }

            Turns++;

            if (!PlaceTypes.ContainsKey(sectorScheme))
            {
                PlaceTypes.Add(sectorScheme, 0);
            }

            PlaceTypes[sectorScheme]++;
        }

        /// <summary>Обнуление текущих очков.</summary>
        public void ResetScores()
        {
            BaseScores = 0;
            Frags.Clear();
            PlaceTypes.Clear();
            Turns = 0;
            Places.Clear();
        }

        public Scores Scores { get; set; }
    }
}
