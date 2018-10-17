using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Props;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на использование предмета инвентаря.
    /// </summary>
    public sealed class UsePropTask : OneTurnActorTaskBase
    {
        [ExcludeFromCodeCoverage]
        public UsePropTask(IActor actor,
            IProp usedProp) : base(actor)
        {
            UsedProp = usedProp;
        }

        public IProp UsedProp { get; }

        protected override void ExecuteTask()
        {
            Actor.UseProp(UsedProp);
        }
    }
}
