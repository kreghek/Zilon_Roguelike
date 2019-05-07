using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zilon.Bot.Models;

namespace Zilon.BotPlayer
{
    class Program
    {
        private static Process process;
        private static TaskCompletionSource<string> tcs;

        static async Task Main(string[] args)
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

            // first
            tcs = new TaskCompletionSource<string>();
            var startDataTask = tcs.Task;
            var startData = await startDataTask;
            var state = JsonConvert.DeserializeObject<State>(startData);
            tcs = null;

            Console.WriteLine($"Current HP: {state.CurrentHp}");
            Console.WriteLine($"Current Position: {state.Position.X}, {state.Position.Y}");

            Console.ReadLine();

            await SendRequestAsync("map");

            Console.ReadLine();

            process.OutputDataReceived -= Process_OutputDataReceived;
            process.Kill();
        }

        private static async Task<string> SendRequestAsync(string reqquest)
        {
            if (tcs != null)
            {
                tcs.SetCanceled();
            }

            tcs = new TaskCompletionSource<string>();

            process.StandardInput.WriteLine(reqquest);

            var task = tcs.Task;
            var data = await task;
            tcs = null;
            return data;

        }

        static string Out;

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"Data from env:{e.Data}");

                if (e.Data == "Enter request:")
                {
                    tcs.SetResult(Out);
                }
                else
                {
                    Out = e.Data;
                }
            }
        }
    }
}
