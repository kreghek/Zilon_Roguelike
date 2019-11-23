using System.Collections.Generic;

namespace Zilon.Core.World
{
    public class GlobeGenerationHistory
    {
        public GlobeGenerationHistory()
        {
            Items = new List<GlobeGenerationHistoryItem>();
        }

        public List<GlobeGenerationHistoryItem> Items { get; }
    }
}
