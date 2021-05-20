﻿using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Common;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие актёров под управлением игрока.
    /// </summary>
    public class TacticalAct : ITacticalAct
    {
        [ExcludeFromCodeCoverage]
        public TacticalAct([NotNull] ITacticalActScheme scheme,
            [NotNull] Roll efficient,
            [NotNull] Roll toHit,
            [CanBeNull] Equipment? equipment)
        {
            Scheme = scheme;

            if (scheme.Stats is null)
            {
                throw new InvalidOperationException();
            }

            Stats = scheme.Stats;

            Efficient = efficient;

            ToHit = toHit;

            Equipment = equipment;

            Constrains = scheme.Constrains;

            CurrentCooldown = scheme.Constrains?.Cooldown != null ? 0 : (int?)null;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Scheme} [{Equipment}]";
        }

        /// <inheritdoc />
        public ITacticalActStatsSubScheme Stats { get; }

        /// <inheritdoc />
        public ITacticalActScheme Scheme { get; }

        /// <inheritdoc />
        public Roll Efficient { get; }

        /// <inheritdoc />
        public Roll ToHit { get; }

        /// <inheritdoc />
        public Equipment? Equipment { get; }

        /// <inheritdoc />
        public ITacticalActConstrainsSubScheme? Constrains { get; }

        /// <inheritdoc />
        public int? CurrentCooldown { get; private set; }

        /// <inheritdoc />
        public void StartCooldownIfItIs()
        {
            CurrentCooldown = Constrains?.Cooldown;
        }

        /// <inheritdoc />
        public void UpdateCooldown()
        {
            if (CurrentCooldown is null)
            {
                // Если КД нет, то ничего не делаем.
                return;
            }

            if (CurrentCooldown > 0)
            {
                CurrentCooldown--;
            }
        }
    }
}