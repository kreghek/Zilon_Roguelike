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

        public virtual bool IsComplete { get; protected set; }

        public virtual int Cost { get => 1000; }

        protected IActorTaskContext Context { get; }

        public abstract void Execute();
    }

    public interface IActorTaskContext
    {
        ISector Sector { get; }
    }

    public sealed class ActorTaskContext : IActorTaskContext
    {
        public ActorTaskContext(ISector sector)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
        }

        public ISector Sector { get; }
    }
}