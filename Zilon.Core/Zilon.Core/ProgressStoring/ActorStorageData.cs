using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.World;

namespace Zilon.Core.ProgressStoring
{
    public sealed class ActorStorageData
    {
        public string PersonId { get; set; }

        public string SectorId { get; set; }

        public OffsetCoords Coords { get; set; }

        public static ActorStorageData Create(IActor actor,
            ISector sector,
            IDictionary<ISector, SectorStorageData> sectorStorageDict,
            IDictionary<IPerson, string> personDict)
        {
            if (actor is null)
            {
                throw new System.ArgumentNullException(nameof(actor));
            }

            if (personDict is null)
            {
                throw new System.ArgumentNullException(nameof(personDict));
            }

            var storageData = new ActorStorageData();

            var hexNode = actor.Node as HexNode;
            storageData.Coords = new OffsetCoords(hexNode.OffsetX, hexNode.OffsetY);
            storageData.PersonId = personDict[actor.Person];

            var sectorStorageData = sectorStorageDict[sector];
            storageData.SectorId = sectorStorageData.Id;

            return storageData;
        }

        public Actor Restore(Globe globe, IActorManager actorManager)
        {
            var person = globe.Persons.Single(x=>x.Id == PersonId);
        }
    }
}
