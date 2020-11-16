using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Zilon.CommonUtilities;

namespace Zilon.BotMassLauncher
{
    internal class Workload
    {
        private readonly ILogger<Workload> _logger;
        private string _botAssembly;
        private string _botCatalog;
        private string _botMode;
        private ulong _infiniteCounter;
        private bool _isInfinite;
        private int _launchCount;
        private string _parallel;
        private string _pathToEnv;
        private string _schemeCatalogPath;
        private string _scorePath;
        private string _scorePrefix;
        private CancellationToken _shutdownToken;
        private CancellationTokenSource _shutdownTokenSource;

        public Workload(ILogger<Workload> logger)
        {
            _logger = logger;
        }

        public void Run(params string[] args)
        {
            _logger.LogTrace("[x] START");

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            _pathToEnv = ArgumentHelper.GetProgramArgument(args, "env");
            _launchCount = int.Parse(ArgumentHelper.GetProgramArgument(args, "launchCount"));
            _scorePrefix = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                                   .Replace(":", "_")
                                   .Replace(".", "_");

            _parallel = ArgumentHelper.GetProgramArgument(args, "parallel");
            _isInfinite = ArgumentHelper.HasProgramArgument(args, "infinite");
            _botMode = ArgumentHelper.GetProgramArgument(args, "mode");
            _scorePath = ArgumentHelper.GetProgramArgument(args, "output");

            _botCatalog = ArgumentHelper.GetProgramArgument(args, "botCatalog");
            _botAssembly = ArgumentHelper.GetProgramArgument(args, "botAssembly");

            _schemeCatalogPath = ArgumentHelper.GetProgramArgument(args, "schemeCatalogPath");

            _shutdownTokenSource = new CancellationTokenSource();
            _shutdownToken = _shutdownTokenSource.Token;

            do
            {
                if (_isInfinite)
                {
                    _logger.LogTrace($"[x] INFINITE COUNTER {_infiniteCounter}");
                }

                if (!string.IsNullOrWhiteSpace(_parallel))
                {
                    RunParallel(int.Parse(_parallel), _logger);
                }
                else
                {
                    RunLinear(_logger);
                }

                _infiniteCounter++;
                if (_infiniteCounter == ulong.MaxValue)
                {
                    _infiniteCounter = 0;
                }
            } while (_isInfinite);

            _logger.LogTrace("[x] COMPLETE");
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (_shutdownTokenSource != null)
            {
                _shutdownTokenSource.Cancel();
            }
        }

        private void RunEnvironment(int iteration, ILogger logger)
        {
            var modeArg = string.Empty;
            if (!string.IsNullOrEmpty(_botMode))
            {
                modeArg = $" mode={_botMode}";
            }

            var infiniteCounterPrefix = string.Empty;
            if (_isInfinite)
            {
                infiniteCounterPrefix = _infiniteCounter + " ";
            }

            logger.LogTrace($"[x] {infiniteCounterPrefix}ITERATION {iteration} STARTED");

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = _pathToEnv,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments =
                        $"serverrun ScorePreffix=\"{_scorePrefix}\"{modeArg} schemeCatalogPath={_schemeCatalogPath} output={_scorePath} botCatalog={_botCatalog} botAssembly={_botAssembly}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                process.Start();

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();

                logger.LogTrace("[x]OUTPUT");
                logger.LogTrace(output);
                logger.LogError("[x]ERROR");
                logger.LogError(error);

                process.WaitForExit(30000);
            }

            logger.LogTrace($"[x] {infiniteCounterPrefix}ITERATION {iteration} FINISHED");
        }

        private void RunLinear(ILogger logger)
        {
            for (var i = 0; i < _launchCount; i++)
            {
                RunEnvironment(i, logger);
            }
        }

        private void RunParallel(int maxDegreeOfParallelism, ILogger logger)
        {
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                CancellationToken = _shutdownToken
            };

            Parallel.For(0, _launchCount, parallelOptions, iteration => RunEnvironment(iteration, logger));
        }
    }
}