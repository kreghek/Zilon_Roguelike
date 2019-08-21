using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.ProgressStoring;
using Zilon.Core.World;

namespace Zilon.Core.WorldGeneration
{
    public class GlobeStorageData
    {
        public TerrainCell[][] Terrain { get; set; }

        public RealmStorageData[] Realms { get; set; }

        public AgentStorageData[] Agents { get; set; }

        public LocalityStorageData[] Localities { get; set; }

        public int AgentCrisys { get; set; }

        public TerrainCell StartProvince { get; set; }

        public TerrainCell HomeProvince { get; set; }


        public static GlobeStorageData Create(Globe globe)
        {
            var storageData = new GlobeStorageData();
            storageData.Terrain = globe.Terrain;

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
                    Coords = locality.Cell.Coords,
                    RealmId = realmDict[locality.Owner].Id,
                    //Population = locality.Population,
                    Branches = locality.Branches.Select(x => new LocalityBranchStorageData { Type = x.Key, Value = x.Value }).ToArray()
                });

            storageData.Localities = localityDict.Select(x => x.Value).ToArray();

            var agentDict = globe.Agents.ToDictionary(agent => agent,
                agent => new AgentStorageData
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = agent.Name,
                    Hp = agent.Hp,
                    RealmId = realmDict[agent.Realm].Id,
                    Location = agent.Location.Coords,
                    Skills = agent.Skills.Select(x => new AgentSkillStorageData
                    {
                        Type = x.Key,
                        Value = x.Value
                    }).ToArray()
                });

            storageData.Agents = agentDict.Select(x => x.Value).ToArray();

            storageData.AgentCrisys = globe.AgentCrisys;

            storageData.HomeProvince = globe.HomeProvince;

            storageData.StartProvince = globe.StartProvince;

            return storageData;
        }

        public Globe Restore()
        {
            var globe = new Globe();

            globe.Terrain = Terrain;

            var realmDict = Realms.ToDictionary(storedRealm => storedRealm.Id, storedRealm => new Realm
            {
                Name = storedRealm.Name,
                Banner = new RealmBanner { MainColor = storedRealm.MainColor }
            });

            globe.Realms = realmDict.Select(x => x.Value).ToList();

            RestoreLocalities(out globe.Localities, out globe.LocalitiesCells, Localities, globe.Terrain, realmDict);

            var agentDict = Agents.ToDictionary(storedAgent => storedAgent.Id);
            RestoreAgents(out globe.Agents, Agents, globe.Terrain, realmDict);

            globe.AgentCrisys = AgentCrisys;
            globe.HomeProvince = globe.Terrain[HomeProvince.Coords.X][HomeProvince.Coords.Y];
            globe.StartProvince = globe.Terrain[StartProvince.Coords.X][StartProvince.Coords.Y];

            return globe;
        }

        private static void RestoreAgents(out List<Agent> agents, AgentStorageData[] storedAgents,
            TerrainCell[][] terrain,
            Dictionary<string, Realm> realmsDict)
        {
            agents = new List<Agent>(storedAgents.Length);

            var flattenTerrain = terrain.SelectMany(x => x).ToArray();
            foreach (var storedAgent in storedAgents)
            {
                var agentCell = terrain[storedAgent.Location.X][storedAgent.Location.Y];
                var agent = new Agent
                {
                    Hp = storedAgent.Hp,
                    Name = storedAgent.Name,
                    Location = agentCell,
                    Realm = realmsDict[storedAgent.RealmId],
                    Skills = storedAgent.Skills.ToDictionary(x => x.Type, x => x.Value)
                };
            }
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
            TerrainCell[][] terrain,
            Dictionary<string, Realm> realmsDict)
        {
            localities = new List<Locality>(storedLocalities.Length);
            localityCells = new Dictionary<TerrainCell, Locality>(storedLocalities.Length);

            var flattenTerrain = terrain.SelectMany(x => x).ToArray();
            foreach (var storedLocality in storedLocalities)
            {
                var localityCell = terrain[storedLocality.Coords.X][storedLocality.Coords.Y];
                var locality = new Locality()
                {
                    Name = storedLocality.Name,
                    Cell = localityCell,
                    Owner = realmsDict[storedLocality.RealmId],
                    //Population = storedLocality.Population,
                    Branches = storedLocality.Branches.ToDictionary(x => x.Type, x => x.Value)
                };

                localities.Add(locality);
                localityCells.Add(localityCell, locality);
            }
        }
    }
}
