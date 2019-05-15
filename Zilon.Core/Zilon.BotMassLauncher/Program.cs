using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Zilon.BotMassLauncher
{
    class Program
    {
        private static string pathToEnv;
        private static int launchCount;
        private static string scorePreffix;

        static void Main()
        {
            pathToEnv = ConfigurationManager.AppSettings["env"];
            launchCount = int.Parse(ConfigurationManager.AppSettings["launchCount"]);
            scorePreffix = DateTime.UtcNow.ToString().Replace(":", "_").Replace(".", "_");

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = 10
            };

            Parallel.For(0, launchCount, parallelOptions, RunEnvironment);

            Console.WriteLine($"[x] COMPLETE");
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
                    Arguments = $"serverrun ScorePreffix=\"{scorePreffix}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };


                process.Start();

                Console.WriteLine(process.StandardOutput.ReadToEnd());

                process.WaitForExit();
            }

            Console.WriteLine($"[x] ITERATION {iteration} FINISHED");
        }
    }
}
