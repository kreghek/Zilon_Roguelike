using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Zilon.Tournament.ApiGate.Models;

namespace Zilon.Tournament.ApiGate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasuresController: Controller
    {
        [HttpGet]
        public ActionResult<IEnumerable<Measure>> Get()
        {
            var resultList = new List<Measure>();

            var outputCatalog = Environment.GetEnvironmentVariable("BOT_OUTPUT_CATALOG");
            var baseName = "BotScores.db3";
            var dbPath = Path.Combine(outputCatalog, baseName);

            var factory = Microsoft.Data.Sqlite.SqliteFactory.Instance;
            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = "Data Source = " + dbPath;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"SELECT [Name],[Mode],[Preffix],[AvgScores] FROM v_measures";
                    command.CommandType = CommandType.Text;
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var measure = ReadSingleRow((IDataRecord)reader);
                        resultList.Add(measure);
                    }
                }
            }

            return resultList;
        }

        private static Measure ReadSingleRow(IDataRecord record)
        {
            return new Measure
            {
                AvgScores = (double)record["AvgScores"]
            };
        }
    }
}
