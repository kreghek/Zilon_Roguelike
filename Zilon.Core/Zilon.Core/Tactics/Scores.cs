using System.Collections.Generic;

using Zilon.Core.Schemes;
using Zilon.Core.World;

namespace Zilon.Core.Tactics
{
    public sealed class Scores
    {
        public Scores()
        {
            Frags = new Dictionary<IMonsterScheme, int>();

            PlaceTypes = new Dictionary<ILocationScheme, int>();

            Places = new HashSet<ProvinceNode>();
        }

        public float TurnCounter { get; set; }

        public int BaseScores { get; set; }

        /// <summary>Фраги по схемам монстров, добытые игроком.</summary>
        public IDictionary<IMonsterScheme, int> Frags { get; set; }

        /// <summary>Счётчик ходов по типам секторов.</summary>
        public IDictionary<ILocationScheme, int> PlaceTypes { get; set; }

        /// <summary>Шаги, прожитые персонажем.</summary>
        public int Turns { get; set; }

        /// <summary>Посещённые места.</summary>
        public ISet<ProvinceNode> Places { get; set; }
        public ScoreAchievements Achievements { get; set; }
    }
}
