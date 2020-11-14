using System.Data;
using System.Data.Common;
using System.IO;

using Zilon.Core.Scoring;

namespace Zilon.BotEnvironment
{
    public static class DatabaseContext
    {
        public static void AppendScores(
            string scorePath,
            IScoreManager scoreManager,
            string botName,
            string scoreFilePreffix,
            string mode,
            string textSummary)
        {
            var fragSum = scoreManager.Frags.Sum(x => x.Value);

            var baseName = "BotScores.db3";
            var dbPath = Path.Combine(scorePath, baseName);
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            var factory = SQLiteFactory.Instance;
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = "Data Source = " + dbPath;
                connection.Open();

                CreateScoresTableIfNotExists(connection);

                CreatMeasuresViewIfNotExists(connection);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        $@"INSERT INTO [Scores](Name, Preffix, Mode, Scores, Turns, Frags, TextSummary)
                    VALUES ('{botName}', '{scoreFilePreffix}', '{mode}', {scoreManager.BaseScores}, {scoreManager.Turns}, {fragSum}, '{textSummary}')";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void CreateScoresTableIfNotExists(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
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
        }

        private static void CreatMeasuresViewIfNotExists(DbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
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
}