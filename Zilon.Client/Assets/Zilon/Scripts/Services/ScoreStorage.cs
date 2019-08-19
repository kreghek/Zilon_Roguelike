using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

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
            var textSummary = CreateTextSummary(scores);
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = $@"INSERT INTO [Scores](Name, Preffix, Mode, Scores, Turns, Frags, TextSummary)
                    VALUES ('{personName}', 'preffix', 'mode', {scores.BaseScores}, {scores.Turns}, {fragSum}, '{textSummary}')";
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    private static string CreateTextSummary(Scores scores)
    {
        var summaryStringBuilder = new StringBuilder();

        summaryStringBuilder.AppendLine("YOU (BOT) DIED");

        summaryStringBuilder.AppendLine($"SCORES: {scores.BaseScores}");

        summaryStringBuilder.AppendLine("=== You survived ===");
        var minutesTotal = scores.Turns * 2;
        var hoursTotal = minutesTotal / 60f;
        var daysTotal = hoursTotal / 24f;
        var days = (int)daysTotal;
        var hours = (int)(hoursTotal - days * 24);

        summaryStringBuilder.AppendLine($"{days} days {hours} hours");
        summaryStringBuilder.AppendLine($"Turns: {scores.Turns}");

        summaryStringBuilder.AppendLine("=== You visited ===");

        summaryStringBuilder.AppendLine($"{scores.Places.Count} places");

        foreach (var placeType in scores.PlaceTypes)
        {
            summaryStringBuilder.AppendLine($"{placeType.Key.Name?.En ?? placeType.Key.Name?.Ru ?? placeType.Key.ToString()}: {placeType.Value} turns");
        }

        summaryStringBuilder.AppendLine("=== You killed ===");
        foreach (var frag in scores.Frags)
        {
            summaryStringBuilder.AppendLine($"{frag.Key.Name?.En ?? frag.Key.Name?.Ru ?? frag.Key.ToString()}: {frag.Value}");
        }

        return summaryStringBuilder.ToString();
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
}
