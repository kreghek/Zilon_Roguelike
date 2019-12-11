using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Core.Commands
{
    public class UpdateGlobeCommand<TContext> : ICommandWrapper<TContext>
    {
        private readonly IGlobeManager _globeManager;
        private readonly IActorTaskSource _botTaskSource;

        public UpdateGlobeCommand(IGlobeManager globeManager,
            IActorTaskSource botTaskSource,
            ICommand<TContext> underlyingCommand)
        {
            _globeManager = globeManager;
            _botTaskSource = botTaskSource;
            UnderlyingCommand = underlyingCommand;
        }

        public ICommand<TContext> UnderlyingCommand { get; }

        public bool CanExecute(TContext context)
        {
            return UnderlyingCommand.CanExecute(context);
        }

        public void Execute(TContext context)
        {
            UnderlyingCommand.Execute(context);

            var globeIterationContext = new GlobeIterationContext(_botTaskSource);
            _globeManager.UpdateGlobeOneStepAsync(globeIterationContext).Wait();
        }
    }
}
