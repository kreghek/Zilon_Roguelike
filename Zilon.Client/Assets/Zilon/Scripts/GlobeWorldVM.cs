using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using Zilon.Logic.Services.Client;

public class GlobeWorldVM : MonoBehaviour
{

    public Map Map;
    public SchemeLocator SchemeLocator;

    public GlobeWorldVM()
    {

    }

    // Use this for initialization
    void Start()
    {
        string connectionString = "URI=file:" + Application.dataPath + "/db/db.bytes";
        using (IDbConnection dbcon = (IDbConnection)new SqliteConnection(connectionString))
        {
            dbcon.Open();

            // Выбираем нужные нам данные
            var sql = "SELECT Id, Value FROM Test";
            using (IDbCommand dbcmd = dbcon.CreateCommand())
            {
                dbcmd.CommandText = sql;
                // Выполняем запрос
                using (IDataReader reader = dbcmd.ExecuteReader())
                {
                    // Читаем и выводим результат
                    while (reader.Read())
                    {
                        Debug.Log($"Id: {reader.GetInt32(0)}, Value: {reader.GetString(1)}");
                    }
                }
            }
            // Закрываем соединение
            dbcon.Close();
        }
    }


    private void Awake()
    {
        var schemeService = new SchemeService(SchemeLocator);
        Map.CreateMapEntities(schemeService, "main");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
