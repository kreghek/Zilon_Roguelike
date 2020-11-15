namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Область действия работы.
    /// </summary>
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum JobScope
    {
        /// <summary>
        /// Общая область действия.
        /// Прогресс не будет сбрасываться после окончания боя.
        /// Не влияет на одноходовые задачи (одним ударом убить 2 противника)
        /// </summary>
        Global,

        /// <summary>
        /// Область действия на высадку.
        /// Если работы в рамках одной высадки не выполнены, то прогресс будет сброшен.
        /// </summary>
        Scenario,

        /// <summary>
        /// Область действия на этап высадки.
        /// Этап высадки это от отдыха до отдыха (ключевые точки).
        /// </summary>
        KeyPoint
    }
}