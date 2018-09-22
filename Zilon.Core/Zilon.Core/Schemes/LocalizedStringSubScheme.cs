using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Локализованная на разные языки строка.
    /// </summary>
    public sealed class LocalizedStringSubScheme
    {
        /// <summary>
        /// Английский вариант.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string En { get; set; }

        /// <summary>
        /// Русский вариант.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public string Ru { get; set; }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"{Ru ?? En}";
        }
    }
}
