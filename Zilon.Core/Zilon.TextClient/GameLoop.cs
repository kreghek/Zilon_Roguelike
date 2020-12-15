using System;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.World;

namespace Zilon.TextClient
{
    internal class GameLoop
    {
        private readonly IGlobe _globe;
        private readonly IPlayer _player;

        public GameLoop(IGlobe globe, IPlayer player)
        {
            _globe = globe ?? throw new ArgumentNullException(nameof(globe));
            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public async Task StartProcessAsync(CancellationToken cancellationToken)
        {
            while (!_player.MainPerson.GetModule<ISurvivalModule>().IsDead)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _globe.UpdateAsync().ConfigureAwait(false);
            }
        }
    }
}