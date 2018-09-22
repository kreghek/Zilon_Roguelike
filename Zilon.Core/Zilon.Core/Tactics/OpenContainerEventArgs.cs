using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Аргументы события на открытие контейнера.
    /// </summary>
    public class OpenContainerEventArgs: EventArgs
    {
        /// <summary>
        /// Результат открытия. True - если открыт успешно. Иначе - false.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public IOpenContainerResult Result { get; }

        [ExcludeFromCodeCoverage]
        public OpenContainerEventArgs(IOpenContainerResult result)
        {
            Result = result;
        }
    }
}
