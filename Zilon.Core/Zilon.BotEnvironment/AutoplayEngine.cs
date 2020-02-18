using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Zilon.Bot.Players;
using Zilon.Bot.Sdk;
using Zilon.CommonUtilities;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Emulation.Common;

namespace Zilon.BotEnvironment
{
    class AutoplayEngine : AutoplayEngineBase<IPluggableActorTaskSource>
    {
        private const string SCORE_FILE_PATH = "bot-scores";
        private const int BOT_EXCEPTION_LIMIT = 3;
        private const int ENVIRONMENT_EXCEPTION_LIMIT = 3;

        private readonly Startup _startup;
        private readonly string scoreFilePreffix;

        int botExceptionCount = 0;
        int envExceptionCount = 0;

        public AutoplayEngine(Startup startup,
            BotSettings botSettings,
            string scoreFilePreffix
            ) : base(botSettings)
        {
            _startup = startup;
            this.scoreFilePreffix = scoreFilePreffix;
        }

        protected override void CatchActorTaskExecutionException(ActorTaskExecutionException exception)
        {
            AppendException(exception, scoreFilePreffix);

            var monsterActorTaskSource = ServiceScope.ServiceProvider.GetRequiredService<MonsterBotActorTaskSource>();
            if (exception.ActorTaskSource != monsterActorTaskSource)
            {
                botExceptionCount++;

                if (botExceptionCount >= BOT_EXCEPTION_LIMIT)
                {
                    AppendFail(ServiceScope.ServiceProvider, scoreFilePreffix);
                    throw exception;
                }
            }
            else
            {
                envExceptionCount++;
                CheckEnvExceptions(envExceptionCount, exception);
                Console.WriteLine($"[.] {exception.Message}");
            }
        }

        protected override void CatchException(Exception exception)
        {
            AppendException(exception, scoreFilePreffix);

            envExceptionCount++;
            CheckEnvExceptions(envExceptionCount, exception);
            Console.WriteLine($"[.] {exception.Message}");
        }

        protected override void ConfigBotAux()
        {
            _startup.ConfigureAux(ServiceScope.ServiceProvider);
        }

        protected override void ProcessEnd()
        {
            var mode = _botSettings.Mode;
            var scoreManager = ServiceScope.ServiceProvider.GetRequiredService<IScoreManager>();
            WriteScores(ServiceScope.ServiceProvider, scoreManager, mode, scoreFilePreffix);
        }

        private static void WriteScores(IServiceProvider serviceFactory, IScoreManager scoreManager, string mode, string scoreFilePreffix)
        {
            var summaryText = TextSummaryHelper.CreateTextSummary(scoreManager.Scores);

            Console.WriteLine(summaryText);

            AppendScores(scoreManager, serviceFactory, scoreFilePreffix, mode, summaryText);
        }

        private static void AppendScores(IScoreManager scoreManager, IServiceProvider serviceFactory, string scoreFilePreffix, string mode, string summary)
        {
            var path = SCORE_FILE_PATH;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var botTaskSource = serviceFactory.GetRequiredService<IPluggableActorTaskSource>();
            var scoreFilePreffixFileName = GetScoreFilePreffix(scoreFilePreffix);
            var filename = Path.Combine(path, $"{botTaskSource.GetType().FullName}{scoreFilePreffixFileName}.scores");
            using (var file = new StreamWriter(filename, append: true))
            {
                var fragSum = scoreManager.Frags.Sum(x => x.Value);
                file.WriteLine($"{DateTime.UtcNow}\t{scoreManager.BaseScores}\t{scoreManager.Turns}\t{fragSum}");
            }

            DatabaseContext.AppendScores(scoreManager, botTaskSource.GetType().FullName, scoreFilePreffix, mode, summary);
        }

        private void AppendException(Exception exception, string scoreFilePreffix)
        {
            var path = SCORE_FILE_PATH;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var botTaskSource = ServiceScope.ServiceProvider.GetRequiredService<IPluggableActorTaskSource>();
            var scoreFilePreffixFileName = GetScoreFilePreffix(scoreFilePreffix);
            var filename = Path.Combine(path, $"{botTaskSource.GetType().FullName}{scoreFilePreffixFileName}.exceptions");
            using (var file = new StreamWriter(filename, append: true))
            {
                file.WriteLine(DateTime.UtcNow);
                file.WriteLine(exception);
                file.WriteLine();
            }
        }

        private static string GetScoreFilePreffix(string scoreFilePreffix)
        {
            var scoreFilePreffixFileName = string.Empty;
            if (!string.IsNullOrWhiteSpace(scoreFilePreffix))
            {
                scoreFilePreffixFileName = $"-{scoreFilePreffix}";
            }

            return scoreFilePreffixFileName;
        }

        private static void AppendFail(IServiceProvider serviceFactory, string scoreFilePreffix)
        {
            Console.WriteLine("[x] Bot task source error limit reached");

            var path = SCORE_FILE_PATH;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var botTaskSource = serviceFactory.GetRequiredService<IPluggableActorTaskSource>();
            var scoreFilePreffixFileName = GetScoreFilePreffix(scoreFilePreffix);
            var filename = Path.Combine(path, $"{botTaskSource.GetType().FullName}{scoreFilePreffixFileName}.scores");
            using (var file = new StreamWriter(filename, append: true))
            {
                file.WriteLine($"-1");
            }
        }

        private static void CheckEnvExceptions(int envExceptionCount, Exception exception)
        {
            if (envExceptionCount >= ENVIRONMENT_EXCEPTION_LIMIT)
            {
                throw exception;
            }
        }
    }
}
