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
            CurrentSector = sector ?? throw new ArgumentNullException(nameof(sector));
            ActiveActor = activeActorViewModel ?? throw new ArgumentNullException(nameof(activeActorViewModel));
            TaskSource = humanActorTaskSource;
            HoverViewModel = hoverViewModel;
            SelectedViewModel = selectedViewModel;
        }

        public ISector CurrentSector { get; }

        public IActorViewModel ActiveActor { get; }

        public IHumanActorTaskSource TaskSource { get; }

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
