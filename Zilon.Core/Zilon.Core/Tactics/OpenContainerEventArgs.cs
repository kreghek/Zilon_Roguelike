using System;

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
        public bool Result { get; }

        public OpenContainerEventArgs(bool result)
        {
            Result = result;
        }
    }
}
