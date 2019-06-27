using System.IO;
using System.Linq;
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
            if (_worldManager.Globe != null)
            {
                SaveGlobe();

                SavePlayer();

                if (_worldManager.Regions != null)
                {
                    foreach (var region in _worldManager.Regions)
                    {
                        SaveGlobeRegion(region.Value, region.Key);
                    }
                }

                if (_humanPlayer.MainPerson != null)
                {
                    SavePerson();
                }
            }
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

            var terrainCells = globe.Terrain.SelectMany(x => x).ToArray();

            foreach (var cell in terrainCells)
            {
                var region =  LoadRegion(cell);
                if (region != null)
                {
                    _worldManager.Regions[cell] = region;
                }
            }

            return true;
        }

        public void SavePerson()
        {
            var storageDataObject = HumanPersonStorageData.Create(_humanPlayer.MainPerson);

            SaveInner(storageDataObject, "Person.txt");
        }

        public bool LoadPerson()
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

        public GlobeRegion LoadRegion(TerrainCell terrainCell)
        {
            var storageDataObject = LoadInner<GlobeRegionStorageData>($"Region{terrainCell}.txt");
            if (storageDataObject == null)
            {
                return null;
            }

            var region = storageDataObject.Restore(_schemeService);

            return region;
        }

        public void SavePlayer()
        {
            var storageData = HumanPlayerStorageData.Create(_humanPlayer);
            SaveInner(storageData, "Player.txt");
        }

        public bool LoadPlayer()
        {
            var storageDataObject = LoadInner<HumanPlayerStorageData>("Player.txt");
            if (storageDataObject == null)
            {
                return false;
            }

            storageDataObject.Restore(_humanPlayer, _worldManager.Globe, _worldManager);

            return true;
        }

        public void Destroy()
        {
            DeleteFile("Globe.txt");
            DeleteFile("Person.txt");
            var regionFiles = Directory.EnumerateFiles(Application.persistentDataPath, "Region*.txt");
            foreach (var regionFile in regionFiles)
            {
                DeleteFile(regionFile);
            }
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

            Debug.Log(dataPath);

            using (var streamReader = File.OpenText(dataPath))
            {
                string jsonString = streamReader.ReadToEnd();
                var storageDataObject = JsonConvert.DeserializeObject<T>(jsonString);
                return storageDataObject;
            }
        }

        private void DeleteFile(string fileName)
        {
            var dataPath = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }
        }
    }
}
