using System;

namespace Zilon.Core.WorldGeneration
{
    public sealed class GlobeGenerationHistoryItem
    {
        public GlobeGenerationHistoryItem(string historyEvent, int iteration)
        {
            Event = historyEvent ?? throw new ArgumentNullException(nameof(historyEvent));
            Iteration = iteration;
        }

        public string Event { get; }
        public int Iteration { get; }
    }
}
