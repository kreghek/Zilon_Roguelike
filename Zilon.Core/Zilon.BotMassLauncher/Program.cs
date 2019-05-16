using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Zilon.BotMassLauncher
{
    class Program
    {
        private static string pathToEnv;
        private static int launchCount;
        private static string scorePreffix;

        static void Main(string[] args)
        {
            pathToEnv = ConfigurationManager.AppSettings["env"];
            launchCount = int.Parse(ConfigurationManager.AppSettings["launchCount"]);
            scorePreffix = DateTime.UtcNow.ToString().Replace(":", "_").Replace(".", "_");

            var parallel = GetProgramArgument(args, "parallel");
            var isInfinite = HasProgramArgument(args, "infinite");
            ulong infiniteCounter = 0;
            do
            {
                if (isInfinite)
                {
                    Console.WriteLine($"[x] INFINITE COUNTER {infiniteCounter}");
                }

                if (!string.IsNullOrWhiteSpace(parallel))
                {
                    RunParallel(int.Parse(parallel));
                }
                else
                {
                    RunLinear();
                }

                infiniteCounter++;
                if (infiniteCounter == ulong.MaxValue)
                {
                    infiniteCounter = 0;
                }
            } while (isInfinite);


            Console.WriteLine($"[x] COMPLETE");
        }

        private static void RunParallel(int maxDegreeOfParallelism)
        {
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };

            Parallel.For(0, launchCount, parallelOptions, RunEnvironment);
        }

        private static void RunLinear()
        {
            for (var i = 0; i < launchCount; i++)
            {
                RunEnvironment(i);
            }
        }

        private static void RunEnvironment(int iteration)
        {
            Console.WriteLine($"[x] ITERATION {iteration} STARTED");

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = pathToEnv,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"serverrun ScorePreffix=\"{scorePreffix}\""
                };


                process.Start();

                process.WaitForExit();
            }

            Console.WriteLine($"[x] ITERATION {iteration} FINISHED");
        }

        private static bool HasProgramArgument(string[] args, string testArg)
        {
            return args?.Select(x => x?.Trim().ToLowerInvariant()).Contains(testArg.ToLowerInvariant()) == true;
        }

        private static string GetProgramArgument(string[] args, string testArg)
        {
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
