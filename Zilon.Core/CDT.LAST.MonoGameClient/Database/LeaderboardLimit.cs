using System;

namespace CDT.LAST.MonoGameClient
{
    internal record LeaderboardLimit
    {
        private const int DEFAULT_GET_LEADERBOARD_LIMIT = 10;

        public LeaderboardLimit(bool areGettingAllRecords = false, int limit = DEFAULT_GET_LEADERBOARD_LIMIT)
        {
            if (areGettingAllRecords && limit != DEFAULT_GET_LEADERBOARD_LIMIT)
            {
                throw new ArgumentException(
                    $"Cannot use options - {nameof(areGettingAllRecords)} = true AND {nameof(limit)} != default value together.");
            }

            AreGettingAllRecords = areGettingAllRecords;
            Limit = limit;
        }

        public bool AreGettingAllRecords { get; init; }

        public int Limit { get; init; }
    }
}