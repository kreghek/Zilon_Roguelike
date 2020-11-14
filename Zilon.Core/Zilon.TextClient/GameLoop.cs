using System;
using System.Threading.Tasks;

namespace Zilon.TextClient
{
    internal class GameLoop
    {
        private readonly IGlobe _globe;

        public GameLoop(IGlobe globe)
        {
            _globe = globe ?? throw new ArgumentNullException(nameof(globe));
        }

        public async Task StartProcessAsync()
        {
            while (true)
            {
                await _globe.UpdateAsync();
            }
        }
    }
}