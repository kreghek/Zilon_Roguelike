using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Core.Commands
{
    public class UpdateGlobeCommand : ICommandWrapper
    {
        private readonly IGlobeManager _globeManager;
        private readonly IActorTaskSource _botTaskSource;

        public UpdateGlobeCommand(IGlobeManager globeManager,
            IActorTaskSource botTaskSource,
            ICommand underlyingCommand)
        {
            _globeManager = globeManager;
            _botTaskSource = botTaskSource;
            UnderlyingCommand = underlyingCommand;
        }

        public ICommand UnderlyingCommand { get; }

        public bool CanExecute()
        {
            return UnderlyingCommand.CanExecute();
        }

        public void Execute()
        {
            UnderlyingCommand.Execute();

            _globeManager.UpdateGlobeOneStepAsync(_botTaskSource).Wait();
        }
    }
}
