using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;

using LightInject;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.BotEnvironment
{
    public static class DatabaseContext
    {
        public static void AppendScores(
            string scorePath,
            IScoreManager scoreManager,
            IServiceFactory serviceFactory,
            string scoreFilePreffix,
            string mode,
            string textSummary)
        {
            var botTaskSource = serviceFactory.GetInstance<IActorTaskSource>("bot");
            var fragSum = scoreManager.Frags.Sum(x => x.Value);

            var baseName = "BotScores.db3";
            var dbPath = Path.Combine(scorePath, baseName);
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }


            var factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
            using (var connection = (SQLiteConnection)factory.CreateConnection())
            {
                connection.ConnectionString = "Data Source = " + dbPath;
                connection.Open();

                CreateScoresTableIfNotExists(connection);

                CreatMeasuresViewIfNotExists(connection);

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = $@"INSERT INTO [Scores](Name, Preffix, Mode, Scores, Turns, Frags, TextSummary)
                    VALUES ('{botTaskSource.GetType().FullName}', '{scoreFilePreffix}', '{mode}', {scoreManager.BaseScores}, {scoreManager.Turns}, {fragSum}, '{textSummary}')";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void CreateScoresTableIfNotExists(SQLiteConnection connection)
        {
            using (var command = new SQLiteCommand(connection))
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

        private static void CreatMeasuresViewIfNotExists(SQLiteConnection connection)
        {
            using (var command = new SQLiteCommand(connection))
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
