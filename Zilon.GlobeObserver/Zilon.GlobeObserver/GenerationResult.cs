using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.WorldGeneration;

namespace Zilon.GlobeObserver
{
    public class GenerationResult
    {
        public GenerationResult(Globe globe, IServiceScope[] scopes)
        {
            Globe = globe ?? throw new ArgumentNullException(nameof(globe));
            Scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        }

        public Globe Globe { get; }
        public IServiceScope[] Scopes { get; }
    }
}
