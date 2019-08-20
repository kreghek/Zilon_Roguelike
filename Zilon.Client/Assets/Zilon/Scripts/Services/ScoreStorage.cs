using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;

using Zilon.Core.ProgressStoring;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    public sealed class ScoreStorage
    {
        public void AppendScores(string personName, Scores scores)
        {
            var pathToDb = Path.Combine(Application.persistentDataPath, "data.bytes");
            var connectionString = $"URI=file:{pathToDb}";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                CreateScoresTableIfNotExists(connection);

                var fragSum = scores.Frags.Sum(x => x.Value);
                var scoreStorageData = ScoresStorageData.Create(scores);
                var summarySerialized = JsonConvert.SerializeObject(scoreStorageData);
                var textSummary = TextSummaryHelper.CreateTextSummary(scores);
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = $@"INSERT INTO [Scores](Name, Preffix, Mode, Scores, Turns, Frags, Summary, TextSummary)
                    VALUES ('{personName}', 'preffix', 'mode', {scores.BaseScores}, {scores.Turns}, {fragSum}, '{summarySerialized}', '{textSummary}')";
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
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                CreateScoresTableIfNotExists(connection);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Name, Scores, TextSummary FROM [Scores] ORDER BY Scores DESC";
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
                                Scores = reader.GetInt32(1)
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
                        Summary TEXT NULL,
                        TextSummary TEXT NULL
                    );";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }
    }
}