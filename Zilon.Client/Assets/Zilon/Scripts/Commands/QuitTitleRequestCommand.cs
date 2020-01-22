using Zenject;

using Zilon.Core.Client.Windows;
using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitTitleRequestCommand : ICommand<ActorModalCommandContext>
    {
        [Inject] ISectorModalManager _sectorModalManager;

        public bool CanExecute(ActorModalCommandContext context)
        {
            return true;
        }

        public void Execute(ActorModalCommandContext context)
        {
            _sectorModalManager.ShowQuitTitleComfirmationModal();
        }
    }
}
