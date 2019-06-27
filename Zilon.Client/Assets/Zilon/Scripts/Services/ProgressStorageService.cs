using System.IO;

using Newtonsoft.Json;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.ProgressStoring;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Assets.Zilon.Scripts.Services
{
    public sealed class ProgressStorageService
    {
        [Inject]
        private readonly IWorldManager _worldManager;

        [Inject]
        private readonly ISchemeService _schemeService;

        [Inject]
        private readonly ISurvivalRandomSource _survivalRandomSource;

        [Inject]
        private IPropFactory _propFactory;

        [Inject]
        private HumanPlayer _humanPlayer;

        public void Save()
        {
            SaveGlobe();
            SavePlayer();
        }

        public void SaveGlobe()
        {
            var storageDataObject = GlobeStorageData.Create(_worldManager.Globe);

            SaveInner(storageDataObject, "Globe.txt");
        }

        public bool LoadGlobe()
        {
            var storageDataObject = LoadInner<GlobeStorageData>("Globe.txt");
            if (storageDataObject == null)
            {
                return false;
            }

            var globe = storageDataObject.Restore();

            _worldManager.Globe = globe;

            return true;
        }

        public void SavePlayer()
        {
            var storageDataObject = HumanPersonStorageData.Create(_humanPlayer.MainPerson);

            SaveInner(storageDataObject, "Person.txt");
        }

        public bool LoadPlayer()
        {
            var storageDataObject = LoadInner<HumanPersonStorageData>("Person.txt");
            if (storageDataObject == null)
            {
                return false;
            }

            var person = storageDataObject.Restore(_schemeService, _survivalRandomSource, _propFactory);

            _humanPlayer.MainPerson = person;

            return true;
        }

        public void SaveGlobeRegion(GlobeRegion globeRegion, TerrainCell terrainCell)
        {
            var strageDataObject = GlobeRegionStorageData.Create(globeRegion, terrainCell);
            SaveInner(strageDataObject, $"Region{terrainCell}.txt");
        }

        public bool LoadRegion(TerrainCell terrainCell)
        {
            var storageDataObject = LoadInner<GlobeRegionNodeStorageData>($"Region{terrainCell}.txt");
            if (storageDataObject == null)
            {
                return false;
            }

            storageDataObject.Restore();
        }

        private void SaveInner(object storageDataObject, string fileName)
        {
            var jsonString = JsonConvert.SerializeObject(storageDataObject, Formatting.Indented);

            var dataPath = Path.Combine(Application.persistentDataPath, fileName);
            using (var streamWriter = File.CreateText(dataPath))
            {
                streamWriter.Write(jsonString);
            }
        }

        private T LoadInner<T>(string fileName) where T : class
        {
            var dataPath = Path.Combine(Application.persistentDataPath, fileName);
            if (!File.Exists(dataPath))
            {
                return null;
            }

            using (var streamReader = File.OpenText(dataPath))
            {
                string jsonString = streamReader.ReadToEnd();
                var storageDataObject = JsonConvert.DeserializeObject<T>(jsonString);
                return storageDataObject;
            }
        }
    }
}
