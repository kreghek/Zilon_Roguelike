using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Базовый класс для всех задач актёра.
    /// </summary>
    /// <remarks>
    /// ОБЯЗАТЕЛЬНО все задачи актёров наследовать от этого класса.
    /// Во избежание ситуации, когда можно забыть инициировать актёра.
    /// </remarks>
    public abstract class ActorTaskBase : IActorTask
    {
        [ExcludeFromCodeCoverage]
        protected ActorTaskBase([NotNull] IActor actor, IActorTaskContext context)
        {
            Actor = actor ?? throw new ArgumentNullException(nameof(actor));
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected IActor Actor { get; }

        protected IActorTaskContext Context { get; }

        public virtual bool IsComplete { get; protected set; }

        public virtual int Cost => 1000;

        public abstract void Execute();
    }
}