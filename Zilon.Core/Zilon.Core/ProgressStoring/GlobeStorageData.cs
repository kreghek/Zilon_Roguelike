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
                    Population = locality.Population,
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

            RestoreLocalities(globe, globe.Terrain, realmDict, Localities);

            return globe;
        }

        private static void RestoreLocalities(Globe globe,
            TerrainCell[][] terrain,
            Dictionary<string, Realm> realmsDict,
            LocalityStorageData[] storedLocalities)
        {
            globe.Localities = new List<Locality>(storedLocalities.Length);
            globe.LocalitiesCells = new Dictionary<TerrainCell, Locality>(storedLocalities.Length);

            var flattenTerrain = terrain.SelectMany(x => x).ToArray();
            foreach (var storedLocality in storedLocalities)
            {
                var localityCell = flattenTerrain.Single(x => x.Coords.X == storedLocality.Coords.X && x.Coords.Y == storedLocality.Coords.Y);
                var locality = new Locality()
                {
                    Name = storedLocality.Name,
                    Cell = localityCell,
                    Owner = realmsDict[storedLocality.RealmId],
                    Population = storedLocality.Population,
                    Branches = storedLocality.Branches.ToDictionary(x => x.Type, x => x.Value)
                };

                globe.Localities.Add(locality);
                globe.LocalitiesCells.Add(localityCell, locality);
            }
        }
    }
}
