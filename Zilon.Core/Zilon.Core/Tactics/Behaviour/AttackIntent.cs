namespace Zilon.Core.Tactics.Behaviour
{
    public class AttackIntent : ICommand
    {
        private IAttackTarget _target;

        public IActor Actor { get; }

        public bool IsComplete { get; set; }

        public void Execute()
        {
            var currentCubePos = Actor.Node.CubeCoords;
            var targetCubePos = _target.Node.CubeCoords;


        }

        public AttackIntent(IActor actor, IAttackTarget target)
        {
            _target = target;
            Actor = actor;
        }
    }
}
