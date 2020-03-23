using System;

namespace Zilon.Core.World
{
    /// <summary>
    /// Стартовый результат генерации мира сервисом генерации.
    /// </summary>
    public sealed class GlobeGenerationResult
    {
        public GlobeGenerationResult(Globe globe, GlobeGenerationHistory history)
        {
            Globe = globe ?? throw new ArgumentNullException(nameof(globe));
            History = history ?? throw new ArgumentNullException(nameof(history));
        }

        public Globe Globe { get; }

        public GlobeGenerationHistory History { get; }
    }
}
