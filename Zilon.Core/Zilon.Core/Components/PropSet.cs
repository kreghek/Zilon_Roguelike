﻿using JetBrains.Annotations;

namespace Zilon.Core.Components
{
    /// <summary>
    /// Набор ресурсов. Используется для крафта, диалогов, прокачки перков.
    /// </summary>
    [PublicAPI]
    public sealed class PropSet
    {
        public int Count { get; set; }
        public string? PropSid { get; set; }
    }
}