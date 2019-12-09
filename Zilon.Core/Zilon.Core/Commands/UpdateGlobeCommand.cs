using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Core.Commands
{
    public class UpdateGlobeCommand : ICommand
    {
        private readonly IGlobeManager _globeManager;
        private readonly IActorTaskSource _botTaskSource;
        private readonly ICommand _underlyingCommand;

        public UpdateGlobeCommand(IGlobeManager globeManager,
            IActorTaskSource botTaskSource,
            ICommand underlyingCommand)
        {
            _globeManager = globeManager;
            _botTaskSource = botTaskSource;
            _underlyingCommand = underlyingCommand;
        }

        public bool CanExecute()
        {
            return _underlyingCommand.CanExecute();
        }

        public void Execute()
        {
            _underlyingCommand.Execute();

            _globeManager.UpdateGlobeOneStepAsync(_botTaskSource).Wait();
        }
    }
}
