using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;

using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

public class ScoreStorage
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
            var summarySerialized = JsonConvert.SerializeObject(scores);
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
