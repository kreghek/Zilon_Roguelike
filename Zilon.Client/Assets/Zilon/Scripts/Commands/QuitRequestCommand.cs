using Zenject;

using Zilon.Core.Client;
using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitRequestCommand : ICommand
    {
        [Inject] ISectorModalManager _sectorModalManager;

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            _sectorModalManager.ShowQuitComfirmationModal();
        }
    }
}
