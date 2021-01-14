using System;
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
            IActorTaskContext context,
            IProp usedProp) : base(actor, context)
        {
            UsedProp = usedProp;
        }

        public IProp UsedProp { get; }

        protected override void ExecuteTask()
        {
            var isAllow = CheckPropRestrictionsNotFired(Actor, Context);

            if (!isAllow)
            {
                throw new InvalidOperationException(
                    $"Attempt to use the prop {UsedProp} which restricted in current context.");
            }

            Actor.UseProp(UsedProp);
        }

        private static bool CheckPropRestrictionsNotFired(IActor actor, IActorTaskContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}