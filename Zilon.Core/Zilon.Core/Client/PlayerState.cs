using System;

using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Client
{
    public class PlayerState : IPlayerState
    {
        private ISelectableViewModel _hoverViewModel;
        private ISelectableViewModel _selectedViewModel;

        /// <summary>Активный актёр.</summary>
        public IActorViewModel ActiveActor { get; set; }

        /// <summary>Выбранный объект.</summary>
        public ISelectableViewModel HoverViewModel
        {
            get => _hoverViewModel;
            set
            {
                _hoverViewModel = value;
                HoverChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>Пользовательский источник задач для актёров.</summary>
        public IHumanActorTaskSource TaskSource { get; set; }

        /// <summary>Зафиксированный выбранный объект.</summary>
        public ISelectableViewModel SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                _selectedViewModel = value;
                _hoverViewModel = value;
            }
        }

        /// <summary>Выстреливает, когда изменяется <see cref="HoverViewModel" />.</summary>
        public event EventHandler HoverChanged;
    }
}
