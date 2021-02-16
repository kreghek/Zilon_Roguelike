using Assets.Zilon.Scripts.Common;
using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine.SceneManagement;

using Zenject;

using Zilon.Core.Commands;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;

namespace Assets.Zilon.Scripts.Commands
{
    sealed class QuitTitleCommand : ICommand
    {
        [Inject]
        [NotNull]
        private readonly IPlayer _player;

        [Inject]
        [NotNull]
        private readonly IHumanActorTaskSource<ISectorTaskSourceContext> _humanActorTaskSource;

        [Inject]
        [NotNull]
        private readonly IAnimationBlockerService _commandBlockerService;

        public bool CanExecute()
        {
            return true;
        }

        public void Execute()
        {
            _humanActorTaskSource.DropIntentionWaiting();
            _commandBlockerService.DropBlockers();
            GameCleanupHelper.ResetState(_player);

            SceneManager.LoadScene("title");
        }
    }
}
