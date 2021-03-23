using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Zilon.Core.PersonModules;
using Zilon.Core.Players;

namespace Zilon.Core.Client.Sector
{
    /// <summary>
    /// Work implementation of the game context.
    /// </summary>
    public sealed class GameLoopContext : IGameLoopContext
    {
        [NotNull]
        private readonly IPlayer _player;

        [NotNull]
        private readonly IInventoryState _inventoryState;

        [NotNull]
        private readonly ISectorUiState _playerState;

        public GameLoopContext(IPlayer player, IInventoryState inventoryState, ISectorUiState playerState)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _inventoryState = inventoryState ?? throw new ArgumentNullException(nameof(inventoryState));
            _playerState = playerState ?? throw new ArgumentNullException(nameof(playerState));
        }

        /// <inheritdoc/>
        public bool HasNextIteration
        {
            get
            {
                var mainPerson = _player.MainPerson;
                if (mainPerson is null)
                {
                    throw new InvalidOperationException("Main person is not defined.");
                }

                var survivalModule = mainPerson.GetModule<ISurvivalModule>();
                var isDead = survivalModule.IsDead;
                return !isDead;
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            var globe = _player.Globe;
            if (globe is null)
            {
                throw new InvalidOperationException("Globe is not defined to update.");
            }

            await globe.UpdateAsync(cancellationToken);
        }
    }
}