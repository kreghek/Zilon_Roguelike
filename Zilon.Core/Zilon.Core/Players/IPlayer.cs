using System;

using Zilon.Core.Persons;
using Zilon.Core.World;

namespace Zilon.Core.Players
{
    public interface IPlayer
    {
        IGlobe Globe { get; }
        IPerson MainPerson { get; }

        [Obsolete("Because we can get in from Globe. Currently remains fro old code.")]
        ISectorNode SectorNode { get; }

        void BindPerson(IGlobe globe, IPerson person);

        void Reset();
    }
}