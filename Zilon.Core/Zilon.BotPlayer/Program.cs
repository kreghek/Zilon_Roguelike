using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zilon.BotPlayer
{
    class Program
    {
        private static Process process;

        static void Main(string[] args)
        {
            var pathToEnv = ConfigurationManager.AppSettings["env"];
            process = new Process
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

            process.OutputDataReceived += Process_OutputDataReceived;

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            Console.ReadLine();
            process.Close();
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"Data from env:{e.Data}");

                if (e.Data == "Enter request:")
                {
                    process.StandardInput.WriteLine("map");
                }
            }
        }
    }
}
