using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Аргументы события на открытие контейнера.
    /// </summary>
    public sealed class OpenContainerEventArgs: EventArgs
    {
        /// <summary>
        /// Результат открытия. True - если открыт успешно. Иначе - false.
        /// </summary>
        public IOpenContainerResult Result { get; }

        [ExcludeFromCodeCoverage]
        public OpenContainerEventArgs([NotNull] IOpenContainerResult result)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }
    }
}
