using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;

using Microsoft.AspNetCore.Mvc;

using Zilon.Tournament.ApiGate.Models;

namespace Zilon.Tournament.ApiGate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasuresController : Controller
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
                    command.CommandText = @"SELECT [Name]
                            ,[Mode]
                            ,MIN([MinScores])*1.0 AS MinScores
                            ,AVG([AvgScores]) AS AvgScores
                            ,MAX([MaxScores])*1.0 AS MaxScores
                            ,MIN([MinTurns])*1.0 AS MinTurns
                            ,AVG([AvgTurns]) AS AvgTurns
                            ,MAX([MaxTurns])*1.0 AS MaxTurns
                            ,MIN([MinFrags])*1.0 AS MinFrags
                            ,AVG([AvgFrags]) AS AvgFrags
                            ,MAX([MaxFrags])*1.0 AS MaxFrags
                            ,AVG([AvgIterationDuration]) AS AvgIterationDuration
                            FROM v_measures
                            GROUP BY [Name], [Mode]
                            ORDER BY AvgScores DESC";
                    command.CommandType = CommandType.Text;
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        using (var diffCommand = connection.CreateCommand())
                        {
                            diffCommand.CommandText = $@"SELECT
                            [MinScores]*1.0   AS MinScores
                            ,[AvgScores]*1.0  AS AvgScores
                            ,[MaxScores]*1.0  AS MaxScores
                            ,[MinTurns]*1.0  AS MinTurns
                            ,[AvgTurns]*1.0  AS AvgTurns
                            ,[MaxTurns]*1.0  AS MaxTurns
                            ,[MinFrags]*1.0  AS MinFrags
                            ,[AvgFrags]*1.0  AS AvgFrags
                            ,[MaxFrags]*1.0  AS MaxFrags
                            ,[AvgIterationDuration]*1.0 AS AvgIterationDuration
                            FROM v_measures WHERE [Name]='{reader["Name"]}' AND [Mode]='{reader["Mode"]}' ORDER BY [Preffix] DESC LIMIT 1 OFFSET 1";
                            diffCommand.CommandType = CommandType.Text;
                            var diffReader = diffCommand.ExecuteReader();

                            var measure = new Measure
                            {
                                BotName = (string)reader["Name"],
                                BotMode = (string)reader["Mode"],

                                MinScores = GetMeasureValue(reader, diffReader, "MinScores"),
                                AvgScores = GetMeasureValue(reader, diffReader, "AvgScores"),
                                MaxScores = GetMeasureValue(reader, diffReader, "MaxScores"),

                                MinTurns = GetMeasureValue(reader, diffReader, "MinTurns"),
                                AvgTurns = GetMeasureValue(reader, diffReader, "AvgTurns"),
                                MaxTurns = GetMeasureValue(reader, diffReader, "MaxTurns"),

                                MinFrags = GetMeasureValue(reader, diffReader, "MinFrags"),
                                AvgFrags = GetMeasureValue(reader, diffReader, "AvgFrags"),
                                MaxFrags = GetMeasureValue(reader, diffReader, "MaxFrags"),

                                AvgIterationDuration = GetMeasureValue(reader, diffReader, "AvgIterationDuration")
                            };

                            resultList.Add(measure);
                        }
                    }
                }
            }

            return resultList;
        }

        private static MeasureValue GetMeasureValue(DbDataReader reader, DbDataReader diffReader, string fieldName)
        {
            var value = GetValue(reader, fieldName);
            var lastValue = GetValue(diffReader, fieldName);
            return new MeasureValue { TotalValue = value, LastValue = lastValue };
        }

        private static double GetValue(DbDataReader diffReader, string fieldName)
        {
            double lastValue = 0;
            if (diffReader.HasRows)
            {
                lastValue = (double)diffReader[fieldName];
            }

            return lastValue;
        }
    }
}