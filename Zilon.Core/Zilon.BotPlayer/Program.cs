using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Zilon.BotPlayer
{
    class Program
    {
        private static BotEnvironment _botEnvironment;

        static async Task Main(string[] args)
        {
            var pathToEnv = ConfigurationManager.AppSettings["env"];
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = pathToEnv,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            // Вызываем до старта процесса, т.к. BatEnvironment
            // в конструкторе подписывается на событие получения вывода.
            _botEnvironment = new BotEnvironment(process);

            process.Start();
            process.BeginOutputReadLine();

            Console.ReadLine();

            var response = await _botEnvironment.RequestAsync("map");
            Console.WriteLine(response);

            Console.ReadLine();
            process.Kill();
        }
    }
}
