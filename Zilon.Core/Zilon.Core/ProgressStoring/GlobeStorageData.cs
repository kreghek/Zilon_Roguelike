using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.ProgressStoring;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
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

            var realmDict = FillRealmsStorageData(globe, storageData);
            FillLocalities(globe, storageData, realmDict);

            var personDict = FillPersons(globe, storageData);

            FillSectors(globe, storageData, personDict);

            return storageData;
        }

        private static void FillSectors(Globe globe, GlobeStorageData storageData, Dictionary<IPerson, string> personDict)
        {
            var sectorStorageDataList = new List<SectorStorageData>();
            foreach (var sectorInfo in globe.SectorInfos)
            {
                var sectorStorageData = SectorStorageData.Create(sectorInfo.Region,
                    sectorInfo.RegionNode,
                    sectorInfo.Sector,
                    personDict);

                sectorStorageDataList.Add(sectorStorageData);
            }

            storageData.Sectors = sectorStorageDataList.ToArray();
        }

        private static Dictionary<IPerson, string> FillPersons(Globe globe, GlobeStorageData storageData)
        {
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
            storageData.Persons = personStorageDataList.ToArray();
            return personDict;
        }

        private static void FillLocalities(Globe globe, GlobeStorageData storageData, Dictionary<Realm, RealmStorageData> realmDict)
        {
            var localityDict = globe.Localities.ToDictionary(locality => locality,
                            locality => new LocalityStorageData
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = locality.Name,
                                RealmId = realmDict[locality.Owner].Id,
                            });

            storageData.Localities = localityDict.Select(x => x.Value).ToArray();
        }

        private static Dictionary<Realm, RealmStorageData> FillRealmsStorageData(Globe globe, GlobeStorageData storageData)
        {
            var realmDict = globe.Realms.ToDictionary(realm => realm, realm => new RealmStorageData
            {
                Id = Guid.NewGuid().ToString(),
                MainColor = realm.Banner.MainColor,
                Name = realm.Name
            });

            storageData.Realms = realmDict.Select(x => x.Value).ToArray();
            return realmDict;
        }

        private static void FillTerrainStorageData(Globe globe, GlobeStorageData storageData)
        {
            var terrainStorageData = TerrainStorageData.Create(globe.Terrain);

            storageData.Terrain = terrainStorageData;
        }

        public Globe Restore(ISchemeService schemeService,
            ISurvivalRandomSource survivalRandomSource,
            IPropFactory propFactory,
            ISectorInfoFactory sectorInfoFactory)
        {
            if (sectorInfoFactory is null)
            {
                throw new ArgumentNullException(nameof(sectorInfoFactory));
            }

            var globe = new Globe();

            RestoreTerrain(globe, schemeService);

            var realmDict = RestoreRealms(globe);

            RestoreLocalities(out globe.Localities, Localities, globe.Terrain, realmDict);

            RestorePersons(globe, schemeService, survivalRandomSource, propFactory);

            RestoreSectors(globe, sectorInfoFactory);

            return globe;
        }

        private Dictionary<string, Realm> RestoreRealms(Globe globe)
        {
            var realmDict = Realms.ToDictionary(storedRealm => storedRealm.Id, storedRealm => new Realm
            {
                Name = storedRealm.Name,
                Banner = new RealmBanner { MainColor = storedRealm.MainColor }
            });

            globe.Realms = realmDict.Select(x => x.Value).ToList();
            return realmDict;
        }

        private void RestoreTerrain(Globe globe, ISchemeService schemeService)
        {
            var terrain = Terrain.Restore(schemeService);
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
            LocalityStorageData[] storedLocalities,
            Terrain terrain,
            Dictionary<string, Realm> realmsDict)
        {
            localities = new List<Locality>(storedLocalities.Length);

            foreach (var storedLocality in storedLocalities)
            {
                var locality = new Locality()
                {
                    Name = storedLocality.Name,
                    Owner = realmsDict[storedLocality.RealmId],
                };

                localities.Add(locality);
            }
        }

        private void RestorePersons(Globe globe, ISchemeService schemeService, ISurvivalRandomSource survivalRandomSource, IPropFactory propFactory)
        {
            globe.Persons = Persons.Select(x => (IPerson)x.Restore(schemeService, survivalRandomSource, propFactory)).ToList();
        }

        private void RestoreSectors(Globe globe, ISectorInfoFactory sectorInfoFactory)
        {
            var infos = new List<SectorInfo>();
            foreach (var sectorInfoStorageData in Sectors)
            {
                var terrainCell = globe.Terrain.Cells.SelectMany(x => x).Single(x => x.Coords == sectorInfoStorageData.TerrainCoords);
                var globeRegion = globe.Terrain.Regions.Single(x => x.TerrainCell == terrainCell);
                var coordX = sectorInfoStorageData.GlobeRegionNodeCoords.X;
                var coordY = sectorInfoStorageData.GlobeRegionNodeCoords.Y;
                var globeRegionNode = globeRegion.RegionNodes.Single(x => x.OffsetX == coordX && x.OffsetY == coordY);

                var info = sectorInfoFactory.Create(globeRegion, globeRegionNode, sectorInfoStorageData);

                infos.Add(info);
            }

            globe.SectorInfos = infos;
        }
    }
}
