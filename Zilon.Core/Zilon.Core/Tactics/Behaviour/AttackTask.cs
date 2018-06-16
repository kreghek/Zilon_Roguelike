namespace Zilon.Core.Tactics.Behaviour
{
    public class AttackTask : IActorTask
    {
        private IAttackTarget _target;

        public IActor Actor { get; }

        public bool IsComplete { get; set; }

        public void Execute()
        {
            var currentCubePos = Actor.Node.CubeCoords;
            var targetCubePos = _target.Node.CubeCoords;


        }

        public AttackTask(IActor actor, IAttackTarget target)
        {
            _target = target;
            Actor = actor;
        }
    }
}
