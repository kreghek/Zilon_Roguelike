using UnityEngine;

using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitCommand : ICommand<ActorModalCommandContext>
    {
        public bool CanExecute(ActorModalCommandContext context)
        {
            return true;
        }

        public void Execute(ActorModalCommandContext context)
        {
            Application.Quit();
        }
    }
}
