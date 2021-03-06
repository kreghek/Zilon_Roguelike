﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Аргументы события на открытие контейнера.
    /// </summary>
    public sealed class OpenContainerEventArgs : EventArgs
    {
        [ExcludeFromCodeCoverage]
        public OpenContainerEventArgs(IStaticObject container, [NotNull] IOpenContainerResult result)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }

        /// <summary>
        /// Контейнер, который пытались открыть.
        /// </summary>
        public IStaticObject Container { get; }

        /// <summary>
        /// Результат открытия. <see cref="SuccessOpenContainerResult">SuccessOpenContainerResult</see>, открытие прошло успешно.
        /// </summary>
        public IOpenContainerResult Result { get; }
    }
}