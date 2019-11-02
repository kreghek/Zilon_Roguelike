using Zenject;

using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitTitleRequestCommand : ICommand
    {
        [Inject] ISectorModalManager _sectorModalManager;

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            _sectorModalManager.ShowQuitTitleComfirmationModal();
        }
    }
}
