using UnityEngine;
using System.Data;
using System.IO;
using System.Data.SQLite;

public class ScoreStorage : MonoBehaviour
{
    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        var connectionString = "URI=file:" + Application.persistentDataPath + "/mydb.db";
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
