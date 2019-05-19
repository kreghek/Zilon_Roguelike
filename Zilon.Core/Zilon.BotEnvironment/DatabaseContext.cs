using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using LightInject;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.BotEnvironment
{
    public static class DatabaseContext
    {
        public static void AppendScores(IScoreManager scoreManager, IServiceFactory serviceFactory, string scoreFilePreffix, string textSummary) {
            var botTaskSource = serviceFactory.GetInstance<IActorTaskSource>("bot");
            var fragSum = scoreManager.Frags.Sum(x => x.Value);



            string baseName = "BotScores.db3";

            SQLiteConnection.CreateFile(baseName);

            var factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
            using (var connection = (SQLiteConnection)factory.CreateConnection())
            {
                connection.ConnectionString = "Data Source = " + baseName;
                connection.Open();

                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = $@"INSERT INTO [Scores](Name, Preffix, Mode, TimeStamp, Score, Turns, Frags, TextSummary)
                    VALUES ('{botTaskSource.GetType().FullName}', '{scoreFilePreffix}', 'mode', '{DateTime.UtcNow}', {scoreManager.BaseScores}, {scoreManager.Turns}, {fragSum}, '{textSummary}')";
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
