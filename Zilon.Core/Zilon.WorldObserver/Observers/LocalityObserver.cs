using System.Linq;
using System.Text;

using Zilon.Core.WorldGeneration;

namespace Zilon.WorldObserver.Observers
{
    class LocalityObserver : IObservableGlobeObject
    {
        private readonly Locality _locality;

        public LocalityObserver(Locality locality)
        {
            _locality = locality;
        }

        public string WriteTextInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Locality: {_locality}");
            sb.AppendLine($"Population: {_locality.CurrentPopulation.Count}");
            sb.AppendLine($"Structures: {_locality.Regions.SelectMany(x=>x.Structures).Count()}");
            sb.AppendLine("Resources Pipeline");
            foreach (var resource in _locality.Stats.ResourcesPipeline)
            {
                sb.AppendLine($"{resource.Key}: {resource.Value}");
            }
            sb.AppendLine("Resources Storage");
            foreach (var resource in _locality.Stats.ResourcesStorage)
            {
                sb.AppendLine($"{resource.Key}: {resource.Value}");
            }

            return sb.ToString();
        }
    }
}
