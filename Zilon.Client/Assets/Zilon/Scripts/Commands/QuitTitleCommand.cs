using UnityEngine.SceneManagement;

using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitTitleCommand : ICommand
    {
        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            SceneManager.LoadScene("title");
        }
    }
}
