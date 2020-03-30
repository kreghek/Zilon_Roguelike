namespace Zilon.Core.Localization
{
    public interface ILocalizedString
    {
        /// <summary>
        /// Английский вариант.
        /// </summary>
        string En { get; }

        /// <summary>
        /// Русский вариант.
        /// </summary>
        string Ru { get; }
    }
}
