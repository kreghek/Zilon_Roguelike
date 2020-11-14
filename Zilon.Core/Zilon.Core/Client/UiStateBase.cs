namespace Zilon.Core.Client
{
    public class UiStateBase : IUiState
    {
        private ISelectableViewModel _hoverViewModel;
        private ISelectableViewModel _selectedViewModel;

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

        /// <summary>Выстреливает, когда изменяется <see cref="HoverViewModel"/>.</summary>
        public event EventHandler HoverChanged;
    }
}