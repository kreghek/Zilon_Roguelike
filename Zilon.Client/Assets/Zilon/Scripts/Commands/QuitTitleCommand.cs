using UnityEngine.SceneManagement;

using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitTitleCommand : ICommand<ActorModalCommandContext>
    {
        public bool CanExecute(ActorModalCommandContext context)
        {
            return true;
        }

        public void Execute(ActorModalCommandContext context)
        {
            SceneManager.LoadScene("title");
        }
    }
}
