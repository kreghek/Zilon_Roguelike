using System;

namespace Zilon.Core.World
{
    /// <summary>
    /// Стартовый результат генерации мира сервисом генерации.
    /// </summary>
    public sealed class GlobeGenerationResult
    {
        public GlobeGenerationResult(Globe globe)
        {
            Globe = globe ?? throw new ArgumentNullException(nameof(globe));
        }

        public Globe Globe { get; }
    }
}
