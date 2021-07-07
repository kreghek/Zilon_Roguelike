namespace CDT.LAST.MonoGameClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;

    using Zilon.Core.Scoring;

    internal record PlayerScore(int Id, string NickName, uint RatingPosition, int Score);

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

    internal class DbContext
    {
        private const string DB_FILE_NAME = "BotScores.db3";

        private const string SCORE_PATH = "player-scores";

        private const string SCORE_FILE_PREFFIX = "";

        private const string MODE = "";

        private const int SCORE_COLUMN_ORDINAL = 2;

        private const int NICKNAME_COLUMN_ORDINAL = 1;

        private const int ID_COLUMN_ORDINAL = 0;

        private readonly string _connectionString = $"Data Source = {DB_FILE_NAME}";

        private readonly SQLiteFactory _dbInstance = SQLiteFactory.Instance;

        private readonly IScoreManager _scoreManager;

        public DbContext(IScoreManager scoreManager)
        {
            _scoreManager = scoreManager;
            if (!Directory.Exists(SCORE_PATH))
            {
                Directory.CreateDirectory(SCORE_PATH);
            }

            var dbPath = Path.Combine(SCORE_PATH, DB_FILE_NAME);
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            CreateTablesIfNotExists();
        }

        public void AppendScores(string playerNickName,
            string textSummary)
        {
            using var connection = _dbInstance.CreateConnection();
            connection.ConnectionString = _connectionString;
            connection.Open();

            var fragSum = _scoreManager.Frags.Sum(x => x.Value);

            using var command = connection.CreateCommand();
            command.CommandText =
                $@"INSERT INTO [Scores](Name, Preffix, Mode, Scores, Turns, Frags, TextSummary)
                    VALUES ('{playerNickName}', '{SCORE_FILE_PREFFIX}', '{MODE}', {_scoreManager.BaseScores}, {_scoreManager.Turns}, {fragSum}, '{textSummary}')";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
            connection.Close();
        }

        public List<PlayerScore> GetAllLeaderboardRecord()
        {
            var wantToGetAllRecords = new LeaderboardLimit(true);
            return GetLeaderBoard(wantToGetAllRecords);
        }

        public List<PlayerScore> GetLeaderBoard(LeaderboardLimit? limit = null)
        {
            limit ??= new LeaderboardLimit();

            using var connection = _dbInstance.CreateConnection();
            connection.ConnectionString = _connectionString;
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = FormatGetLeaderboardQuery(limit);
            command.CommandType = CommandType.Text;

            using var sqlReader = command.ExecuteReader();
            var scores = new List<PlayerScore>();
            while (sqlReader.Read())
            {
                var scoreId = sqlReader.GetInt32(ID_COLUMN_ORDINAL);
                var nickName = sqlReader.GetString(NICKNAME_COLUMN_ORDINAL);
                var score = sqlReader.GetInt32(SCORE_COLUMN_ORDINAL);
                var playerScore = new PlayerScore(scoreId, nickName, 0, score);
                scores.Add(playerScore);
            }

            connection.Close();

            var ratedScores = scores.Select(
                (score, rating) => score with
                {
                    RatingPosition = (uint)rating + 1
                }).ToList();

            return ratedScores;
        }

        private static void CreateScoresTableIfNotExists(DbConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"CREATE TABLE IF NOT EXISTS [Scores](
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Preffix TEXT NULL,
                        Mode TEXT NULL,
                        TimeStamp TEXT NOT NULL DEFAULT (datetime(current_timestamp)),
                        Scores INTEGER NULL,
                        Turns INTEGER NULL,
                        Frags INTEGER NULL,
                        TextSummary TEXT NULL
                    );";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }

        private void CreateTablesIfNotExists()
        {
            using var connection = _dbInstance.CreateConnection();
            connection.ConnectionString = _connectionString;
            connection.Open();

            CreateScoresTableIfNotExists(connection);

            CreatMeasuresViewIfNotExists(connection);
            connection.Close();
        }

        private static void CreatMeasuresViewIfNotExists(DbConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"CREATE VIEW IF NOT EXISTS [v_Measures] ([Name]
                            ,[Preffix]
                            ,[Mode]
                            ,[MinScores]
                            ,[AvgScores]
                            ,[MaxScores]
                            ,[MinTurns]
                            ,[AvgTurns]
                            ,[MaxTurns]
                            ,[MinFrags]
                            ,[AvgFrags]
                            ,[MaxFrags]
                            ,[Iterations]
                            ,[AvgIterationDuration])
                        AS
                        SELECT [Name]
                            ,[Preffix]
                            ,[Mode]
                            ,MIN([Scores]) AS MinScores
                            ,ROUND(AVG([Scores]), 1) AS AvgScores
                            ,MAX([Scores]) AS MaxScores
                            ,MIN([Turns]) AS MinTurns
                            ,ROUND(AVG([Turns]), 1) AS AvgTurns
                            ,MAX([Turns]) AS MaxTurns
                            ,MIN([Frags]) AS MinFrags
                            ,ROUND(AVG([Frags]), 1) AS AvgFrags
                            ,MAX([Frags]) AS MaxFrags
                            ,COUNT(*) AS Iterations
                            ,ROUND((julianday(MAX([TimeStamp])) - julianday(MIN([TimeStamp]))) * 24 * 60 * 60 / COUNT(*), 1) AS AvgIterationDuration
                        FROM [Scores]
                        GROUP BY [Name] ,[Preffix] ,[Mode]";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }

        private static string FormatGetLeaderboardQuery(LeaderboardLimit limit)
        {
            const string BASE_QUERY = "SELECT Id, Name, Scores FROM [Scores] ORDER BY Scores DESC";
            if (limit.AreGettingAllRecords)
            {
                return BASE_QUERY;
            }

            var limitQuery = $"{BASE_QUERY} LIMIT {limit.Limit}";

            return limitQuery;
        }
    }
}