using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;

namespace Zilon.Core.Commands
{
    public class NextTurnCommand : ActorCommandBase
    {
        private readonly IDecisionSource _decisionSource;

        public override bool CanExecute(SectorCommandContext context)
        {
            return true;
        }

        protected override void ExecuteTacticCommand(SectorCommandContext context)
        {
            var intention = new Intention<IdleTask>(actor => new IdleTask(actor, _decisionSource));
            context.TaskSource.Intent(intention);
        }

        [ExcludeFromCodeCoverage]
        public NextTurnCommand(
            IDecisionSource decisionSource) :
            base()
        {
            _decisionSource = decisionSource;
        }
    }
}