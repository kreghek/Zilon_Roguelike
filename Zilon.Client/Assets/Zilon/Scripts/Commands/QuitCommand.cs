using UnityEngine;

using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitCommand : ICommand
    {
        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            Application.Quit();
        }
    }
}
