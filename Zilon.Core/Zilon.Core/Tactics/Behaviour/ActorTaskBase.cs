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
        protected ActorTaskBase([NotNull] IActor actor)
        {
            Actor = actor ?? throw new System.ArgumentNullException(nameof(actor));
        }

        protected IActor Actor { get; }

        public virtual bool IsComplete { get; protected set; }
        public virtual bool IsComplete { get; protected set; }
        public int Cost { get => 1000; }

        public abstract void Execute();
    }
}
