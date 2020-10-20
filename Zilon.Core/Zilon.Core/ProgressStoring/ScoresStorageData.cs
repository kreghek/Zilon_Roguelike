using System.Linq;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.ProgressStoring
{
    public sealed class ScoresStorageData
    {
        public float TurnCounter { get; set; }

        public int BaseScores { get; set; }

        /// <summary>Фраги по схемам монстров, добытые игроком.</summary>
        public ScoreSidCounterStorageData[] Frags { get; set; }

        /// <summary>Счётчик ходов по типам секторов.</summary>
        public ScoreSidCounterStorageData[] PlaceTypes { get; set; }

        /// <summary>Шаги, прожитые персонажем.</summary>
        public int Turns { get; set; }

        /// <summary>Посещённые места.</summary>
        public OffsetCoords[] Places { get; set; }
        public ScoreAchievements Achievements { get; set; }

        public static ScoresStorageData Create(Scores scores)
        {
            var storageData = new ScoresStorageData();

            storageData.Achievements = scores.Achievements;
            storageData.BaseScores = scores.BaseScores;
            storageData.Frags = scores.Frags.Select(x => new ScoreSidCounterStorageData { Sid = x.Key.Sid, Value = x.Value }).ToArray();
            //storageData.Places = scores.Places.Select(x => new OffsetCoords(x.OffsetX, x.OffsetY)).ToArray();
            storageData.PlaceTypes = scores.PlaceTypes.Select(x => new ScoreSidCounterStorageData { Sid = x.Key.Sid, Value = x.Value }).ToArray();
            storageData.TurnCounter = scores.TurnCounter;
            storageData.Turns = scores.Turns;

            return storageData;
        }

        public Scores Restore(ISchemeService schemeService)
        {
            var scores = new Scores();

            scores.Achievements = Achievements;
            scores.BaseScores = BaseScores;
            scores.Frags = Frags.ToDictionary(x => schemeService.GetScheme<IMonsterScheme>(x.Sid), x => x.Value);
            scores.PlaceTypes = PlaceTypes.ToDictionary(x => schemeService.GetScheme<ILocationScheme>(x.Sid), x => x.Value);
            scores.TurnCounter = TurnCounter;
            scores.Turns = Turns;

            return scores;
        }
    }
}