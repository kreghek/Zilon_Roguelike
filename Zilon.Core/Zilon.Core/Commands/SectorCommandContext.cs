using System;

using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    public class SectorCommandContext
    {
        public SectorCommandContext(
            ISector sector,
            IActorViewModel activeActorViewModel,
            IHumanActorTaskSource humanActorTaskSource,
            ISelectableViewModel hoverViewModel,
            ISelectableViewModel selectedViewModel)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            ActiveActorViewModel = activeActorViewModel ?? throw new ArgumentNullException(nameof(activeActorViewModel));
            HumanActorTaskSource = humanActorTaskSource;
            HoverViewModel = hoverViewModel;
            SelectedViewModel = selectedViewModel;
        }

        public ISector Sector { get; }

        public IActorViewModel ActiveActorViewModel { get; }
        public IHumanActorTaskSource HumanActorTaskSource { get; }

        /// <summary>
        /// Выбранный объект.
        /// </summary>
        public ISelectableViewModel HoverViewModel { get; }

        /// <summary>
        /// Зафиксированный выбранный объект.
        /// </summary>
        public ISelectableViewModel SelectedViewModel { get; }
    }
}
