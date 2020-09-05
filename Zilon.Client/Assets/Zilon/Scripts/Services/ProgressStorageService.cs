using System;
using System.IO;

using Newtonsoft.Json;

using UnityEngine;

using Zenject;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.ProgressStoring;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;

namespace Assets.Zilon.Scripts.Services
{
    public sealed class ProgressStorageService
    {
        [Inject]
        private readonly ISchemeService _schemeService;

        [Inject]
        private readonly ISurvivalRandomSource _survivalRandomSource;

        [Inject]
        private readonly IPropFactory _propFactory;

        [Inject]
        private readonly IPlayer _humanPlayer;

        [Inject]
        private readonly IScoreManager _scoreManager;

        public bool HasSaves()
        {
            var dataPath = Path.Combine(Application.persistentDataPath, "Globe.txt");
            return File.Exists(dataPath);
        }

        public void Save()
        {
            SavePlayer();
            SaveScores();

            if (_humanPlayer.MainPerson != null)
            {
                SavePerson();
            }
        }

        public void SavePerson()
        {
            //var storageDataObject = HumanPersonStorageData.Create(_humanPlayer.MainPerson);

            //SaveInner(storageDataObject, "Person.txt");
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

        public void SavePlayer()
        {
            //TODO Отключено после ввода Globe.
            // Этот код перестал быть совместим. Поэтому отключен пока емля не закрутиться
            // (не будет запущен рабочий билд с Globe)
            //var storageData = HumanPlayerStorageData.Create(_humanPlayer);
            //SaveInner(storageData, "Player.txt");
            throw new NotImplementedException();
        }

        public bool LoadPlayer()
        {
            //TODO Отключено после ввода Globe.
            // Этот код перестал быть совместим. Поэтому отключен пока емля не закрутиться
            // (не будет запущен рабочий билд с Globe)

            //var storageDataObject = LoadInner<HumanPlayerStorageData>("Player.txt");
            //if (storageDataObject == null)
            //{
            //    return false;
            //}

            //storageDataObject.Restore(_humanPlayer);

            //LoadScores();

            //return true;
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            DeleteFile("Person.txt");
            DeleteFile("Player.txt");
            DeleteFile("Scores.txt");
            var regionFiles = Directory.EnumerateFiles(Application.persistentDataPath, "Region*.txt");
            foreach (var regionFile in regionFiles)
            {
                DeleteFile(regionFile);
            }
        }

        public void SaveScores()
        {
            var storageDataObject = ScoresStorageData.Create(_scoreManager.Scores);

            SaveInner(storageDataObject, "Scores.txt");
        }

        public bool LoadScores()
        {
            var storageDataObject = LoadInner<ScoresStorageData>("Scores.txt");
            if (storageDataObject == null)
            {
                return false;
            }

            var score = storageDataObject.Restore(_schemeService);

            _scoreManager.Scores = score;

            return true;
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
