using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Zilon.BotPlayer
{
    public class BotEnvironment : IBotEnvironment, IDisposable
    {
        private readonly Process _process;

        private readonly StringBuilder _outStringBuilder;

        private TaskCompletionSource<string> _tcs;

        public BotEnvironment(Process process)
        {
            _process = process;
            _outStringBuilder = new StringBuilder();

            _process.OutputDataReceived += Process_OutputDataReceived;
        }

        public void Dispose()
        {
            _process.OutputDataReceived -= Process_OutputDataReceived;

            if (_tcs != null)
            {
                _tcs.SetCanceled();
            }
        }

        public async Task<string> RequestAsync(string requestText)
        {
            if (_tcs != null)
            {
                _tcs.SetCanceled();
            }

            _tcs = new TaskCompletionSource<string>();

            _process.StandardInput.WriteLine(requestText);

            var task = _tcs.Task;
            var data = await task;
            _tcs = null;
            return data;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Debug.WriteLine($"Data from env:{e.Data}");

                if (e.Data == "Enter request:")
                {
                    // TODO Это условие неправильное. Потому что оно поглощает первый запрос при запуске приложении. Переделать.
                    if (_tcs != null)
                    {
                        _tcs.SetResult(_outStringBuilder.ToString());
                        _outStringBuilder.Clear();
                    }
                }
                else
                {
                    _outStringBuilder.Append(e.Data);
                }
            }
        }
    }
}
