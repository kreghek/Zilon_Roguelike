using System.Threading.Tasks;

using Newtonsoft.Json;

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
        public Task SaveAsync(Globe globe, string name)
        {
            return Task.Run(() =>
            {
                var globeStorageData = GlobeStorageData.Create(globe);
                var globeStorageDataSerialized = JsonConvert.SerializeObject(globeStorageData);

                System.IO.File.WriteAllText($"{name}.json", globeStorageDataSerialized);
            });
        }
    }
}
