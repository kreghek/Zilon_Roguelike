namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема для хранения ограничений на действие.
    /// </summary>
    /// <remarks>
    /// Тут будет:
    /// 1. требования к патронам (PropResource*).
    /// 2. КД (Cooldown).
    /// 3. Выболнение условий типа, использовать сразу после убийства.
    /// 4. Ограничение на использование в течении определённого времени (в течении боя, UsageResource*)
    /// </remarks>
    public interface ITacticalActConstrainsSubScheme : ISubScheme
    {
        /// <summary>
        /// Тип ресурсов, необходимых для выполнения действия.
        /// </summary>
        string PropResourceType { get; }

        /// <summary>
        /// Количество ресурсов, необходимых для использования действия.
        /// </summary>
        int? PropResourceCount { get; }
    }
}