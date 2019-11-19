using System;

using Zilon.Core.WorldGeneration;

namespace Zilon.GlobeObserver
{
    public class GenerationResult
    {
        public GenerationResult(Globe globe)
        {
            Globe = globe ?? throw new ArgumentNullException(nameof(globe));
        }

        public Globe Globe { get; }
    }
}
