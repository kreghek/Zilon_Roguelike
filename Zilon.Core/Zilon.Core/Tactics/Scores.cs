using System.Collections.Generic;

using Zilon.Core.Diseases;
using Zilon.Core.Schemes;

namespace Zilon.Core.Tactics
{
    public sealed class Scores
    {
        public Scores()
        {
            Frags = new Dictionary<IMonsterScheme, int>();

            PlaceTypes = new Dictionary<ILocationScheme, int>();

            Diseases = new List<IDisease>();
        }

        public float TurnCounter { get; set; }

        public int BaseScores { get; set; }

        /// <summary>Фраги по схемам монстров, добытые игроком.</summary>
        public IDictionary<IMonsterScheme, int> Frags { get; set; }

        /// <summary>Счётчик ходов по типам секторов.</summary>
        public IDictionary<ILocationScheme, int> PlaceTypes { get; set; }

        /// <summary>Шаги, прожитые персонажем.</summary>
        public int Turns { get; set; }

        /// <summary>
        /// Различные достижения, полученные заигровую сессию.
        /// </summary>
        public ScoreAchievements Achievements { get; set; }

        /// <summary>
        /// Болезни, которыми был инфицирован персонаж.
        /// </summary>
        public IList<IDisease> Diseases { get; }
    }
}