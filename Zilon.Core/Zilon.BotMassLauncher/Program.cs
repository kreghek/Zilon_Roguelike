using System;
using System.Configuration;
using System.Diagnostics;

namespace Zilon.BotMassLauncher
{
    class Program
    {
        static void Main()
        {
            var pathToEnv = ConfigurationManager.AppSettings["env"];
            var launchCount = int.Parse(ConfigurationManager.AppSettings["launchCount"]);

            for (var i = 0; i < launchCount; i++)
            {
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = pathToEnv,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Arguments = "serverrun",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };


                    process.Start();

                    Console.WriteLine(process.StandardOutput.ReadToEnd());

                    process.WaitForExit();
                }
            }
        }
    }
}
