using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

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
            var isAllow = UsePropHelper.CheckPropAllowedByRestrictions(UsedProp, Actor, Context);

            if (!isAllow)
            {
                throw new InvalidOperationException(
                    $"Attempt to use the prop {UsedProp} which restricted in current context.");
            }

            Actor.UseProp(UsedProp);
        }
    }
}