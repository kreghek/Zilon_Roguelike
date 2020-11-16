using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.ProgressStoring
{
    public sealed class ActorStorageData
    {
        public OffsetCoords Coords { get; set; }

        public string PersonId { get; set; }

        public string SectorId { get; set; }

        public static ActorStorageData Create(
            IActor actor,
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

            var hexNode = (HexNode)actor.Node;
            storageData.Coords = hexNode.OffsetCoords;
            storageData.PersonId = personDict[actor.Person];

            return storageData;
        }
    }
}