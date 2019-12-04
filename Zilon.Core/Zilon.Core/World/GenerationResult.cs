using System;

namespace Zilon.Core.World
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
