using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на использование предмета инвентаря.
    /// </summary>
    public sealed class UsePropTask : OneTurnActorTaskBase
    {
        private readonly IProp _usedProp;

        public UsePropTask(IActor actor,
            IProp usedProp) : base(actor)
        {
            _usedProp = usedProp;
        }

        protected override void ExecuteTask()
        {
            Actor.UseProp(_usedProp);
        }
    }
}
