using System.Threading.Tasks;

using Zilon.Core.WorldGeneration;

namespace Zilon.GlobeObserver
{
    public interface IGlobeStorage
    {
        Task Save(Globe globe, string name);
    }
}
