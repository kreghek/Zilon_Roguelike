using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Core.Commands;
using Zilon.Core.Players;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitTitleCommand : ICommand
    {
        [Inject]
        [NotNull]
        private readonly IPlayer _player;

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            _player.Reset();

            SceneManager.LoadScene("title");
        }
    }
}
