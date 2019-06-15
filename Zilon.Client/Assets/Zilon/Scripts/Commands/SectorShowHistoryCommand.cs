using Zenject;

using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    internal sealed class SectorShowHistoryCommand : ICommand
    {
        [Inject]
        private ISectorModalManager _sectorModalManager;

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            _sectorModalManager.ShowHistoryBookModal();
        }
    }
}
