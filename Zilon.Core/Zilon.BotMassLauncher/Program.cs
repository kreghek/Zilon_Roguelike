using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Zilon.BotMassLauncher
{
    class Program
    {
        private static string _pathToEnv;
        private static int _launchCount;
        private static string _scorePreffix;
        private static string _parallel;
        private static bool _isInfinite;
        private static ulong _infiniteCounter;
        private static string _botMode;
        private static CancellationToken _shutdownToken;
        private static CancellationTokenSource _shutdownTokenSource;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            //TODO Восстановить работу с конфигами или переделать на чтение аргументов.
            // Стало нерабочим после портирования на netcore.
            //_pathToEnv = ConfigurationManager.AppSettings["env"];
            //_launchCount = int.Parse(ConfigurationManager.AppSettings["launchCount"]);
            _scorePreffix = DateTime.UtcNow.ToString().Replace(":", "_").Replace(".", "_");

            _parallel = GetProgramArgument(args, "parallel");
            _isInfinite = HasProgramArgument(args, "infinite");
            _botMode = GetProgramArgument(args, "mode");

            _shutdownTokenSource = new CancellationTokenSource();
            _shutdownToken = _shutdownTokenSource.Token;

            do
            {
                if (_isInfinite)
                {
                    Console.WriteLine($"[x] INFINITE COUNTER {_infiniteCounter}");
                }

                if (!string.IsNullOrWhiteSpace(_parallel))
                {
                    RunParallel(int.Parse(_parallel));
                }
                else
                {
                    RunLinear();
                }

                _infiniteCounter++;
                if (_infiniteCounter == ulong.MaxValue)
                {
                    _infiniteCounter = 0;
                }
            } while (_isInfinite);


            Console.WriteLine($"[x] COMPLETE");
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (_shutdownTokenSource != null)
            {
                _shutdownTokenSource.Cancel();
            }
        }

        private static void RunParallel(int maxDegreeOfParallelism)
        {
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = _shutdownToken
            };

            Parallel.For(0, _launchCount, parallelOptions, RunEnvironment);
        }

        private static void RunLinear()
        {
            for (var i = 0; i < _launchCount; i++)
            {
                RunEnvironment(i);
            }
        }

        private static void RunEnvironment(int iteration)
        {
            var modeArg = string.Empty;
            if (!string.IsNullOrEmpty(_botMode))
            {
                modeArg = $" mode={_botMode}";
            }

            var infiniteCounterPreffix = string.Empty;
            if (_isInfinite)
            {
                infiniteCounterPreffix = _infiniteCounter.ToString() + " ";
            }

            Console.WriteLine($"[x] {infiniteCounterPreffix}ITERATION {iteration} STARTED");

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = _pathToEnv,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"serverrun ScorePreffix=\"{_scorePreffix}\"{modeArg}"
                };

                process.Start();

                process.WaitForExit(30000);
            }

            Console.WriteLine($"[x] {infiniteCounterPreffix}ITERATION {iteration} FINISHED");
        }

        private static bool HasProgramArgument(string[] args, string testArg)
        {
            return args?.Select(x => x?.Trim().ToLowerInvariant()).Contains(testArg.ToLowerInvariant()) == true;
        }

        private static string GetProgramArgument(string[] args, string testArg)
        {
            if (args == null)
            {
                return null;
            }

            foreach (var arg in args)
            {
                var components = arg.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (string.Equals(components[0], testArg, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (components.Length >= 2)
                    {
                        return components[1];
                    }
                }
            }

            return null;
        }
    }
}
