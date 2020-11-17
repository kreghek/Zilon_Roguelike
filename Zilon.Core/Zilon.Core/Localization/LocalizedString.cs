namespace Zilon.Core.Localization
{
    /// <summary>
    /// Базовая реализация локализованной стоки.
    /// </summary>
    public class LocalizedString : ILocalizedString
    {
        /// <inheritdoc />
        public string En { get; set; }

        /// <inheritdoc />
        public string Ru { get; set; }
    }
}