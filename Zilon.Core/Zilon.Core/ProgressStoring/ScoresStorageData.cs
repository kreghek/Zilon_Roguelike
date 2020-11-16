using System.Linq;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.ProgressStoring
{
    public sealed class ScoresStorageData
    {
        public ScoreAchievements Achievements { get; set; }

        public int BaseScores { get; set; }

        /// <summary>Фраги по схемам монстров, добытые игроком.</summary>
        public ScoreSidCounterStorageData[] Frags { get; set; }

        /// <summary>Посещённые места.</summary>
        public OffsetCoords[] Places { get; set; }

        /// <summary>Счётчик ходов по типам секторов.</summary>
        public ScoreSidCounterStorageData[] PlaceTypes { get; set; }

        public float TurnCounter { get; set; }

        /// <summary>Шаги, прожитые персонажем.</summary>
        public int Turns { get; set; }

        public static ScoresStorageData Create(Scores scores)
        {
            if (scores is null)
            {
                throw new System.ArgumentNullException(nameof(scores));
            }

            var storageData = new ScoresStorageData
            {
                Achievements = scores.Achievements,
                BaseScores = scores.BaseScores,
                Frags =
                    scores.Frags.Select(x => new ScoreSidCounterStorageData
                    {
                        Sid = x.Key.Sid,
                        Value = x.Value
                    })
                          .ToArray(),
                PlaceTypes =
                    scores.PlaceTypes.Select(x => new ScoreSidCounterStorageData
                    {
                        Sid = x.Key.Sid,
                        Value = x.Value
                    })
                          .ToArray(),
                TurnCounter = scores.TurnCounter,
                Turns = scores.Turns
            };

            return storageData;
        }

        public Scores Restore(ISchemeService schemeService)
        {
            var scores = new Scores
            {
                Achievements = Achievements,
                BaseScores = BaseScores,
                Frags = Frags.ToDictionary(x => schemeService.GetScheme<IMonsterScheme>(x.Sid), x => x.Value),
                PlaceTypes =
                    PlaceTypes.ToDictionary(x => schemeService.GetScheme<ILocationScheme>(x.Sid), x => x.Value),
                TurnCounter = TurnCounter,
                Turns = Turns
            };

            return scores;
        }
    }
}