using System.Threading.Tasks;

using Zilon.Core.WorldGeneration;

namespace Zilon.GlobeObserver
{
    public interface IGlobeStorage
    {
        Task SaveAsync(Globe globe, string name);
    }
}
