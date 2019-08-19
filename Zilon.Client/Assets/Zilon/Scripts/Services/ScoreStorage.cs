using System.Data.SQLite;
using System.IO;

using UnityEngine;

public class ScoreStorage : MonoBehaviour
{
    void Start()
    {
        var pathToDb = Path.Combine(Application.persistentDataPath, "data.bytes");
        var connectionString = $"URI=file:{pathToDb}";
        var connection = new SQLiteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "CREATE TABLE IF NOT EXISTS 'highscores' ( " +
                          "  'id' INTEGER PRIMARY KEY, " +
                          "  'name' TEXT NOT NULL, " +
                          "  'score' INTEGER NOT NULL" +
                          ");";
        command.ExecuteNonQuery();
        connection.Close();
    }
}
