using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;

namespace Assets.Zilon.Scripts.Models.Sector
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
                var survivalModule = _player.MainPerson.GetModule<ISurvivalModule>();
                var isDead = survivalModule.IsDead;
                return !isDead;
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAsync(CancellationToken cancellationToken)
        {
            await _player.Globe.UpdateAsync(cancellationToken);

            ClearupActionUiState();
        }

        private void ClearupActionUiState()
        {
            _inventoryState.SelectedProp = null;
            _playerState.SelectedViewModel = null;
        }
    }
}