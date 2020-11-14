namespace Zilon.Core.Localization
{
    /// <summary>
    ///     Локализованная строка на разных языках.
    /// </summary>
    public interface ILocalizedString
    {
        /// <summary>
        ///     Английский вариант.
        /// </summary>
        string En { get; }

        /// <summary>
        ///     Русский вариант.
        /// </summary>
        string Ru { get; }
    }
}