namespace Zilon.Core.Tactics.Behaviour
{
    public class AttackIntention : IIntention
    {
        private readonly IAttackTarget _target;
        private readonly ITacticalActUsageService _tacticalActUsageService;

        public AttackIntention(IAttackTarget target, ITacticalActUsageService tacticalActUsageService)
        {
            _target = target;
            _tacticalActUsageService = tacticalActUsageService;
        }

        public IActorTask CreateActorTask(IActorTask currentTask, IActor actor)
        {
            var task = new AttackTask(actor, _target, _tacticalActUsageService);
            return task;
        }
    }
}
