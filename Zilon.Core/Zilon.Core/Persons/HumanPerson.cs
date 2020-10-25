using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Zilon.Core.PersonModules;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж, находящийся под управлением игрока.
    /// </summary>
    public class HumanPerson : PersonBase
    {
        /// <inheritdoc/>
        public override int Id { get; set; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IPersonScheme Scheme { get; }

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        public override PhysicalSize PhysicalSize { get => PhysicalSize.Size1; }

        public HumanPerson([NotNull] IPersonScheme scheme, IFraction fraction) : base(fraction)
        {
            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));

            Name = scheme.Sid;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name}";
        }
    }

    public interface IFowData : IPersonModule
    {
        ISectorFowData GetSectorFowData(ISector sector);
    }

    public class FowData : IFowData
    {
        private readonly IDictionary<ISector, ISectorFowData> _sectorFows;

        public FowData()
        {
            _sectorFows = new Dictionary<ISector, ISectorFowData>();
        }

        public string Key { get => nameof(FowData); }
        public bool IsActive { get; set; }

        public ISectorFowData GetSectorFowData(ISector sector)
        {
            if (!_sectorFows.TryGetValue(sector, out var sectorFowData))
            {
                sectorFowData = new HumanSectorFowData();
                _sectorFows[sector] = sectorFowData;
            }

            return sectorFowData;
        }
    }
}