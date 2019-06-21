using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

namespace Assets.Zilon.Scripts.Services
{
    public sealed class ProgressStorageService
    {
        [Inject]
        private readonly IWorldManager _worldManager;

        public void Save(Globe globe)
        {
            var storageDataObject = GlobeStorageData.Create(globe);

            var jsonString = JsonUtility.ToJson(storageDataObject);

            var dataPath = Path.Combine(Application.persistentDataPath, "Globe.txt");
            using (var streamWriter = File.CreateText(dataPath))
            {
                streamWriter.Write(jsonString);
            }
        }

        public bool Load()
        {
            var dataPath = Path.Combine(Application.persistentDataPath, "Globe.txt");
            if (!File.Exists(dataPath))
            {
                return false;
            }

            using (StreamReader streamReader = File.OpenText(dataPath))
            {
                string jsonString = streamReader.ReadToEnd();
                var storageDataObject = JsonUtility.FromJson<GlobeStorageData>(jsonString);
                var globe = storageDataObject.Restore();

                _worldManager.Globe = globe;
            }

            return true;
        }
    }
}
