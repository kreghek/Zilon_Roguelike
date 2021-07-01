namespace CDT.LAST.MonoGameClient
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;

    using Zilon.Core.Scoring;

    internal record PlayerScore(string Id, string NickName, uint RatingPosition, int Score);

    internal class DbContext
    {
        private const string BaseName = "BotScores.db3";

        private readonly IScoreManager _scoreManager;

        public DbContext(IScoreManager scoreManager)
        {
            _scoreManager = scoreManager;
        }

        public void AppendScores(
            string scorePath,
            string botName,
            string scoreFilePreffix,
            string mode,
            string textSummary)
        {
            if (!Directory.Exists(scorePath))
                Directory.CreateDirectory(scorePath);

            var dbPath = Path.Combine(scorePath, BaseName);
            if (!File.Exists(dbPath))
                SQLiteConnection.CreateFile(dbPath);

            var factory = SQLiteFactory.Instance;
            using var connection = factory.CreateConnection();
            connection.ConnectionString = "Data Source = " + dbPath;
            connection.Open();

            CreateScoresTableIfNotExists(connection);

            CreatMeasuresViewIfNotExists(connection);

            var fragSum = _scoreManager.Frags.Sum(x => x.Value);

            using var command = connection.CreateCommand();
            command.CommandText =
                $@"INSERT INTO [Scores](Name, Preffix, Mode, Scores, Turns, Frags, TextSummary)
                    VALUES ('{botName}', '{scoreFilePreffix}', '{mode}', {_scoreManager.BaseScores}, {_scoreManager.Turns}, {fragSum}, '{textSummary}')";
            command.CommandType = CommandType.Text;
            command.ExecuteNonQuery();
        }

        public List<PlayerScore> GetLeaderBoard(int limit = 10)
        {
            return new List<PlayerScore>
            {
                new PlayerScore("333", "Warcru", 1, 9999), new PlayerScore("1234", "Solo", 2, 322)
            };
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
    }
}