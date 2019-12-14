using System;

using Zilon.Core.Client;
using Zilon.Core.Tactics;

namespace Zilon.Core.Commands
{
    public sealed class ActorModalCommandContext
    {
        public ActorModalCommandContext(
            IActor actor,
            ISelectableViewModel hoverViewModel,
            ISelectableViewModel selectedViewModel)
        {
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
            HoverViewModel = hoverViewModel;
            SelectedViewModel = selectedViewModel;
        }

        public IActor Actor { get; }

        /// <summary>
        /// Выбранный объект.
        /// </summary>
        ISelectableViewModel HoverViewModel { get; }

        /// <summary>
        /// Зафиксированный выбранный объект.
        /// </summary>
        ISelectableViewModel SelectedViewModel { get; }
    }
}
