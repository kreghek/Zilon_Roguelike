﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using Zilon.Core.PersonModules;
using Zilon.Core.Players;

namespace Zilon.Core.Client.Sector
{
    /// <summary>
    /// Work implementation of the game context.
    /// </summary>
    public sealed class GlobeLoopContext : IGlobeLoopContext
    {
        private readonly IPlayer _player;

        [ExcludeFromCodeCoverage]
        public GlobeLoopContext(IPlayer player)
        {
            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
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

        /// <inheritdoc />
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