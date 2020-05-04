using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using JetBrains.Annotations;

using Newtonsoft.Json;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.LogicCalculations;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons.Auxiliary;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Персонаж, находящийся под управлением игрока.
    /// </summary>
    public class HumanPerson : PersonBase
    {
        private readonly ITacticalActScheme _defaultActScheme;
        private readonly ISurvivalRandomSource _survivalRandomSource;

        /// <inheritdoc/>
        public override int Id { get; set; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IPersonScheme Scheme { get; }

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        public override PhysicalSize PhysicalSize { get => PhysicalSize.Size1; }

        public HumanPerson([NotNull] IPersonScheme scheme,
            [NotNull] ITacticalActScheme defaultActScheme) : base()
        {
            _defaultActScheme = defaultActScheme ?? throw new ArgumentNullException(nameof(defaultActScheme));

            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));

            Name = scheme.Sid;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}