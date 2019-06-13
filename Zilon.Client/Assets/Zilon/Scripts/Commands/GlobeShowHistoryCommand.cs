using Zenject;

using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    internal sealed class GlobeShowHistoryCommand : ICommand
    {
        [Inject]
        private IGlobeModalManager _globeModalManager;

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            _globeModalManager.ShowHistoryBookModal();
        }
    }
}
