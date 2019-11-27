using System.Threading.Tasks;

using Newtonsoft.Json;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.World;
using Zilon.Core.World;

namespace Zilon.GlobeObserver
{
    public class GlobeStorage : IGlobeStorage
    {
        private readonly ISchemeService _schemeService;
        private readonly ISurvivalRandomSource _survivalRandomSource;
        private readonly IPropFactory _propFactory;
        private readonly ISectorInfoFactory _sectorInfoFactory;

        public GlobeStorage(ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            ISectorInfoFactory sectorInfoFactory)
        {
            _schemeService = schemeService;
            _survivalRandomSource = survivalRandomSource;
            _propFactory = propFactory;
            _sectorInfoFactory = sectorInfoFactory;
        }

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

                using (var file = System.IO.File.CreateText($"{name}.json"))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(file, globeStorageData);
                }
            });
        }

        public Task<Globe> LoadAsync(string name)
        {
            return Task.Run(() =>
            {
                using (var file = System.IO.File.OpenText($"{name}.json"))
                {
                    var serializer = new JsonSerializer();
                    var globeStorageData = (GlobeStorageData)serializer.Deserialize(file, typeof(GlobeStorageData));

                    var globe = globeStorageData.Restore(_schemeService, _survivalRandomSource, _propFactory, _sectorInfoFactory);

                    return globe;
                }
            });
        }

        public bool HasFile(string name)
        {
            return System.IO.File.Exists($"{name}.json");
        }
    }
}
