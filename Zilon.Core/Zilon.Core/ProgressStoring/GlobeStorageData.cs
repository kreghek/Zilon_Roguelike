using System;
using System.Linq;

using Zilon.Core.ProgressStoring;

namespace Zilon.Core.WorldGeneration
{
    public class GlobeStorageData
    {
        public TerrainCell[][] Terrain { get; set; }

        public RealmStorageData[] Realms { get; set; }

        public AgentStorageData[] Agents { get; set; }

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


    }
}
