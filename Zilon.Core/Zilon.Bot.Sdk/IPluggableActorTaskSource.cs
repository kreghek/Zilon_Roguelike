using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Bot.Sdk
{
    /// <summary>
    ///     Интерфейс источника команд для динамического подключения.
    /// </summary>
    public interface IPluggableActorTaskSource<TContext> : ISectorActorTaskSource<TContext>
        where TContext : ISectorTaskSourceContext
    {
    }
}