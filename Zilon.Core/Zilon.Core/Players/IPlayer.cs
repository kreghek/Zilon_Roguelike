using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Persons;
using Zilon.Core.World;

namespace Zilon.Core.Players
{
    public interface IPlayer
    {
        IGlobe? Globe { get; }
        IPerson? MainPerson { get; }

        [Obsolete("Because we can get in from Globe. Currently remains fro old code.")]
        [ExcludeFromCodeCoverage]
        ISectorNode SectorNode { get; }

        void BindPerson(IGlobe globe, IPerson person);

        void Reset();
    }
}