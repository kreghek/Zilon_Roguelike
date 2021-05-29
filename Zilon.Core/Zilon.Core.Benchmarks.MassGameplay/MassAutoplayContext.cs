using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Persons;
using Zilon.Core.World;
using Zilon.Emulation.Common;

namespace Zilon.Core.Benchmarks.MassGameplay
{
    internal sealed class MassAutoplayContext : IAutoplayContext
    {
        private readonly IGlobe _globe;
        private IPerson? _currentFollowedPerson;

        public MassAutoplayContext(IGlobe globe)
        {
            _currentFollowedPerson = GetAvailableFollowedPerson(globe);
            _globe = globe;
        }

        private static bool CheckFollowedFraction(IPerson person)
        {
            return person.Fraction == Fractions.Pilgrims;
        }

        private static IPerson? GetAvailableFollowedPerson(IGlobe globe)
        {
            var followedPerson = globe.SectorNodes
                .Where(x => x.Sector != null)
                //Code smell. Because there are no ways to say that sector is not null.
                .Select(x => x.Sector!)
                .SelectMany(x => x.ActorManager.Items)
                .Select(x => x.Person)
                .FirstOrDefault(x => CheckFollowedFraction(x) && !x.CheckIsDead());

            return followedPerson;
        }

        public async Task<bool> CheckNextIterationAsync()
        {
            return await Task.Run(() =>
            {
                if (_currentFollowedPerson is null || _currentFollowedPerson.CheckIsDead())
                {
                    var possibleFollowedPerson = GetAvailableFollowedPerson(_globe);
                    if (possibleFollowedPerson is null)
                    {
                        return false;
                    }

                    _currentFollowedPerson = possibleFollowedPerson;

                    // There is guaratee the selected person is alive.
                    // See method GetAvailableFollowedPerson().
                    return true;
                }

                return true;
            });
        }
    }
}