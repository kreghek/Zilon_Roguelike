using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.ProgressStoring;
using Zilon.Core.World;

namespace Zilon.Core.WorldGeneration
{
    public class GlobeStorageData
    {
        /// <summary>
        /// Полная информация о ландшафте мира.
        /// </summary>
        public TerrainStorageData Terrain { get; set; }

        /// <summary>
        /// Информация о текущих государствах мира.
        /// </summary>
        public RealmStorageData[] Realms { get; set; }

        /// <summary>
        /// Информация о текущих населённых пунктах мира.
        /// </summary>
        public LocalityStorageData[] Localities { get; set; }

        public SectorStorageData[] Sectors { get; set; }

        public HumanPersonStorageData[] Persons { get; set; }

        public static GlobeStorageData Create(Globe globe)
        {
            if (globe is null)
            {
                throw new ArgumentNullException(nameof(globe));
            }

            var storageData = new GlobeStorageData();
            FillTerrainStorageData(globe, storageData);

            var realmDict = globe.Realms.ToDictionary(realm => realm, realm => new RealmStorageData
            {
                Id = Guid.NewGuid().ToString(),
                MainColor = realm.Banner.MainColor,
                Name = realm.Name
            });

            storageData.Realms = realmDict.Select(x => x.Value).ToArray();

            var localityDict = globe.Localities.ToDictionary(locality => locality,
                locality => new LocalityStorageData
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = locality.Name,
                    RealmId = realmDict[locality.Owner].Id,
                });

            storageData.Localities = localityDict.Select(x => x.Value).ToArray();

            var sectorStorageDataList = new List<SectorStorageData>();
            var personStorageDataList = new List<HumanPersonStorageData>();
            var personDict = new Dictionary<IPerson, string>();
            foreach (var sectorInfo in globe.SectorInfos)
            {
                foreach (var actor in sectorInfo.ActorManager.Items)
                {
                    var personStorageData = HumanPersonStorageData.Create(actor.Person as HumanPerson);
                    personStorageDataList.Add(personStorageData);
                    personDict.Add(actor.Person, personStorageData.Id);
                }
            }

            foreach (var sectorInfo in globe.SectorInfos)
            {
                var sectorStorageData = SectorStorageData.Create(sectorInfo.Sector, personDict);
                sectorStorageDataList.Add(sectorStorageData);
            }

            storageData.Persons = personStorageDataList.ToArray();
            storageData.Sectors = sectorStorageDataList.ToArray();

            return storageData;
        }

        private static void FillTerrainStorageData(Globe globe, GlobeStorageData storageData)
        {
            var terrainStorageData = TerrainStorageData.Create(globe.Terrain);

            storageData.Terrain = terrainStorageData;
        }

        public Globe Restore()
        {
            var globe = new Globe();

            RestoreTerrain(globe);

            var realmDict = Realms.ToDictionary(storedRealm => storedRealm.Id, storedRealm => new Realm
            {
                Name = storedRealm.Name,
                Banner = new RealmBanner { MainColor = storedRealm.MainColor }
            });

            globe.Realms = realmDict.Select(x => x.Value).ToList();

            RestoreLocalities(out globe.Localities, out globe.LocalitiesCells, Localities, globe.Terrain, realmDict);

            return globe;
        }

        private void RestoreTerrain(Globe globe)
        {
            var terrain = Terrain.Restore();
            globe.Terrain = terrain;
        }

        /// <summary>
        /// Восстанавливает нас.пункты в указанные коллекции.
        /// </summary>
        /// <param name="localities"> Целевая коллекция населённых пунктов. </param>
        /// <param name="localityCells"> Соответствующая целевая коллекция кеша узлов населённых пунктов. </param>
        /// <param name="storedLocalities"> Данные сохранения по нас.пунктам. </param>
        /// <param name="terrain"> Территория мира. </param>
        /// <param name="realmsDict"> Словарь государств. Нужен, чтобы знать id государств, которые были в файле сохранения. </param>
        private static void RestoreLocalities(out List<Locality> localities,
            out Dictionary<TerrainCell, Locality> localityCells,
            LocalityStorageData[] storedLocalities,
            Terrain terrain,
            Dictionary<string, Realm> realmsDict)
        {
            localities = new List<Locality>(storedLocalities.Length);
            localityCells = new Dictionary<TerrainCell, Locality>(storedLocalities.Length);

            foreach (var storedLocality in storedLocalities)
            {
                var localityCell = terrain.Cells[storedLocality.Coords.X][storedLocality.Coords.Y];
                var locality = new Locality()
                {
                    Name = storedLocality.Name,
                    Owner = realmsDict[storedLocality.RealmId],
                };

                localities.Add(locality);
                localityCells.Add(localityCell, locality);
            }
        }
    }
}
