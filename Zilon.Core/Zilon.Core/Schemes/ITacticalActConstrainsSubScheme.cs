﻿namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема для хранения ограничений на действие.
    /// </summary>
    /// <remarks>
    /// Доступные/потенциальные ограничения:
    /// 1. требования к патронам (PropResource*).
    /// 2. КД (Cooldown).
    /// 3. Выполнение условий типа, использовать сразу после убийства.
    /// 4. Ограничение на использование в течении определённого времени (в течении боя, UsageResource*)
    /// </remarks>
    public interface ITacticalActConstrainsSubScheme : ISubScheme
    {
        /// <summary>
        /// КД на использование.
        /// Это количество ходов, которое не доступно действие после применения.
        /// </summary>
        int? Cooldown { get; }

        int? EnergyCost { get; }

        /// <summary>
        /// Количество ресурсов, необходимых для использования действия.
        /// </summary>
        int? PropResourceCount { get; }

        /// <summary>
        /// Тип ресурсов, необходимых для выполнения действия.
        /// </summary>
        string? PropResourceType { get; }
    }
}