using System.Collections.Generic;

namespace Zilon.Core.World
{
    public class RealmRelations
    {
        public RealmRelations()
        {
            Relations = new Dictionary<Realm, RealmRelationType>();
        }

        public Dictionary<Realm, RealmRelationType> Relations { get; set; }
    }
}
