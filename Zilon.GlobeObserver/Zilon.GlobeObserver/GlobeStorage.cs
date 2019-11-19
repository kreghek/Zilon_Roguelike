using System.Threading.Tasks;
using Zilon.Core.WorldGeneration;

namespace Zilon.GlobeObserver
{
    public class GlobeStorage : IGlobeStorage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="globeState"></param>
        /// <param name="name"> Наименование мира. </param>
        public Task Save(Globe globe, string name)
        {
            return Task.CompletedTask;
        }
    }
}
