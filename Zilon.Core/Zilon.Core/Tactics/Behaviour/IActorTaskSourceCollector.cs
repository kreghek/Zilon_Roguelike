namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    ///     Сервис для сбора всех источников команд, используемых в системе.
    /// </summary>
    public interface IActorTaskSourceCollector
    {
        IActorTaskSource<ISectorTaskSourceContext>[] GetCurrentTaskSources();
    }
}