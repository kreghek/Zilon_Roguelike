//using Mono.Data.Sqlite;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Schemes;

public class GlobeWorldVM : MonoBehaviour
{
    [NotNull] [Inject] private readonly ISchemeService _schemeService;
    
    public GlobeMapVM Map;
    public Text Text;

    public GlobeWorldVM()
    {
    }

    // Use this for initialization
    void Start()
    {
        //try
        //{
        //    var dbAsset = Resources.Load<TextAsset>("Db/db");

        //    Text.text = $"Db size: {dbAsset.bytes.Length}";

        //    var fs = File.Open(Application.dataPath + "/db.bytes", FileMode.Create);
        //    BinaryWriter binary = new BinaryWriter(fs);
        //    binary.Write(dbAsset.bytes);
        //    fs.Close();

        //    string connectionString = "URI=file:" + Application.dataPath + "/db.bytes";
        //    using (IDbConnection dbcon = new SqliteConnection(connectionString))
        //    {
        //        dbcon.Open();

        //        // Выбираем нужные нам данные
        //        var sql = "SELECT Id, Value FROM Test";
        //        using (IDbCommand dbcmd = dbcon.CreateCommand())
        //        {
        //            dbcmd.CommandText = sql;
        //            // Выполняем запрос
        //            using (IDataReader reader = dbcmd.ExecuteReader())
        //            {
        //                // Читаем и выводим результат
        //                while (reader.Read())
        //                {
        //                    Text.text += $"Id: {reader.GetInt32(0)}, Value: {reader.GetString(1)}";
        //                    Debug.Log($"Id: {reader.GetInt32(0)}, Value: {reader.GetString(1)}");
        //                }
        //            }
        //        }

        //        var sqlInsert = "INSERT INTO Test(Value) VALUES('auto')";
        //        using (IDbCommand dbcmd = dbcon.CreateCommand())
        //        {
        //            dbcmd.CommandText = sqlInsert;
        //            // Выполняем запрос
        //            dbcmd.ExecuteNonQuery();
        //        }

        //        // Закрываем соединение
        //        dbcon.Close();
        //    }
        //}
        //catch (Exception e)
        //{
        //    Text.text += e.ToString();
        //}
    }


    private void Awake()
    {
        Map.CreateMapEntities(_schemeService, "main");
    }

    // Update is called once per frame
    void Update()
    {
    }
}