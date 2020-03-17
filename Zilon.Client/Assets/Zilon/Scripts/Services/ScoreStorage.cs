using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using Mono.Data.SqliteClient;

using Newtonsoft.Json;

using UnityEngine;

using Zilon.Core.ProgressStoring;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    public sealed class ScoreStorage
    {
        public void AppendScores(string personName, Scores scores, string deathReason)
        {
            var pathToDb = Path.Combine(Application.persistentDataPath, "data.bytes");
            var connectionString = $"URI=file:{pathToDb}";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                CreateScoresTableIfNotExists(connection);

                var fragSum = scores.Frags.Sum(x => x.Value);
                var scoreStorageData = ScoresStorageData.Create(scores);
                var summarySerialized = JsonConvert.SerializeObject(scoreStorageData);
                var textSummary = TextSummaryHelper.CreateTextSummary(scores);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"INSERT INTO [Scores](Name, Preffix, Mode, Scores, DeathReason, Turns, Frags, Summary, TextSummary)
                    VALUES ('{personName}', 'preffix', 'mode', {scores.BaseScores}, '{deathReason}', {scores.Turns}, {fragSum}, '{summarySerialized}', '{textSummary}')";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        public ScoresRecord[] ReadScores()
        {
            var recordList = new List<ScoresRecord>();

            var pathToDb = Path.Combine(Application.persistentDataPath, "data.bytes");
            var connectionString = $"URI=file:{pathToDb}";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                CreateScoresTableIfNotExists(connection);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Name, Scores, DeathReason, TextSummary FROM [Scores] ORDER BY Scores DESC";
                    command.CommandType = CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        var number = 1;

                        while (reader.Read())
                        {
                            var record = new ScoresRecord
                            {
                                Number = number,
                                Name = reader.GetString(0),
                                Scores = reader.GetInt32(1),
                                DeathReason = reader.GetString(2)
                            };

                            recordList.Add(record);

                            number++;
                        }
                    }
                }
                connection.Close();
            }

            return recordList.ToArray();
        }

        public AggregateScores ReadAggregateScores()
        {
            // По умолчанию все значения пустые.
            // Предположительно, это будет использовать в самом начале игры, когда нет записей.
            var aggregateScores = new AggregateScores();

            var pathToDb = Path.Combine(Application.persistentDataPath, "data.bytes");
            var connectionString = $"URI=file:{pathToDb}";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                CreateScoresTableIfNotExists(connection);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT
                            MIN([Scores]) AS MinScores
                            ,ROUND(AVG([Scores]), 1) AS AvgScores
                            ,MAX([Scores]) AS MaxScores
                            ,MIN([Turns]) AS MinTurns
                            ,ROUND(AVG([Turns]), 1) AS AvgTurns
                            ,MAX([Turns]) AS MaxTurns
                            ,MIN([Frags]) AS MinFrags
                            ,ROUND(AVG([Frags]), 1) AS AvgFrags
                            ,MAX([Frags]) AS MaxFrags
                        FROM [Scores]
                        GROUP BY [Preffix] ,[Mode]";
                    command.CommandType = CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            aggregateScores = new AggregateScores
                            {
                                AvgScores = reader.GetFloat(1),
                                MaxScores = reader.GetFloat(2),
                                AvgTurns = reader.GetFloat(4),
                                MaxTurns = reader.GetFloat(5),
                                AvgFrags = reader.GetFloat(7),
                                MaxFrags = reader.GetFloat(8)
                            };
                        }
                    }
                }
                connection.Close();
            }

            return aggregateScores;
        }

        private static void CreateScoresTableIfNotExists(SqliteConnection connection)
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
                        DeathReason TEXT NOT NULL,
                        Turns INTEGER NULL,
                        Frags INTEGER NULL,
                        Summary TEXT NULL,
                        TextSummary TEXT NULL
                    );";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }
    }
}