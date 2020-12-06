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

        [NotNull]
        [Inject]
        private readonly GlobeStorage _globeStorage;

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            _player.Reset();
            _globeStorage.Reset();

            SceneManager.LoadScene("title");
        }
    }
}
