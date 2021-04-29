using UnityEngine;

using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitCommand : ICommand
    {
        public CanExecuteCheckResult CanExecute()
        {
            return CanExecuteCheckResult.CreateSuccessful();
        }

        public void Execute()
        {
            Application.Quit();
        }
    }
}
