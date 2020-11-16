using System;

namespace Zilon.Core.Client
{
    public interface IUiState
    {
        /// <summary>
        /// Выбранный объект.
        /// </summary>
        ISelectableViewModel HoverViewModel { get; set; }

        /// <summary>
        /// Зафиксированный выбранный объект.
        /// </summary>
        ISelectableViewModel SelectedViewModel { get; set; }

        /// <summary>Выстреливает, когда изменяется <see cref="HoverViewModel" />.</summary>
        event EventHandler HoverChanged;
    }
}