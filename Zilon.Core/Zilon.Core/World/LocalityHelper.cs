using System.Linq;
using Zilon.Core.World;

namespace Zilon.Core.WorldGeneration
{
    public static class LocalityHelper
    {
        public static bool LimitIsReached(Agent agent, Globe globe) {
            var globeSize = globe.Terrain.Count() * globe.Terrain[0].Count();
            var realmLocalitiesLimit = globeSize / globe.Realms.Count;

            // Можем выполнять захват, если не превышен лимит городов текущего государства.
            var realmLocalities = globe.Localities.Where(x => x.Owner == agent.Realm);
            if (realmLocalities.Count() >= realmLocalitiesLimit)
            {
                return true;
            }

            return false;
        }
    }
}
