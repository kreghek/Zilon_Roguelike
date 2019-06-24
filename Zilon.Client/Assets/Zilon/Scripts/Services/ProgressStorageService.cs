using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void SaveGlobe(Globe globe)
        {
            var storageDataObject = GlobeStorageData.Create(globe);

            var jsonString = JsonConvert.SerializeObject(storageDataObject);

            var dataPath = Path.Combine(Application.persistentDataPath, "Globe.txt");
            using (var streamWriter = File.CreateText(dataPath))
            {
                streamWriter.Write(jsonString);
            }
        }

        public bool LoadGlobe()
        {
            var dataPath = Path.Combine(Application.persistentDataPath, "Globe.txt");
            if (!File.Exists(dataPath))
            {
                return false;
            }

            using (var streamReader = File.OpenText(dataPath))
            {
                string jsonString = streamReader.ReadToEnd();
                var storageDataObject = JsonConvert.DeserializeObject<GlobeStorageData>(jsonString);
                var globe = storageDataObject.Restore();

                _worldManager.Globe = globe;
            }

            return true;
        }


        public void SavePlayer(HumanPlayer player)
        {
            var storageDataObject = HumanPersonStorageData.Create(player.MainPerson);

            var jsonString = JsonConvert.SerializeObject(storageDataObject);

            var dataPath = Path.Combine(Application.persistentDataPath, "Person.txt");
            using (var streamWriter = File.CreateText(dataPath))
            {
                streamWriter.Write(jsonString);
            }
        }

        public bool LoadPlayer()
        {
            var dataPath = Path.Combine(Application.persistentDataPath, "Person.txt");
            if (!File.Exists(dataPath))
            {
                return false;
            }

            using (var streamReader = File.OpenText(dataPath))
            {
                string jsonString = streamReader.ReadToEnd();
                var storageDataObject = JsonConvert.DeserializeObject<HumanPersonStorageData>(jsonString);
                var person = storageDataObject.Restore(_schemeService, _survivalRandomSource, _propFactory);

                _humanPlayer.MainPerson = person;
            }

            return true;
        }
    }
}
