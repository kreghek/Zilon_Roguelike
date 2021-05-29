using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.World;
using Zilon.Emulation.Common;

namespace Zilon.Core.Benchmarks.Move
{
    internal sealed class MassAutoplayContext : IAutoplayContext
    {
        private readonly IGlobe _globe;
        private IPerson _currentFollowedPerson;

        public MassAutoplayContext(IGlobe globe)
        {
            _currentFollowedPerson = GetAvailableFollowedPerson(globe);
            _globe = globe;
        }

        private static IPerson GetAvailableFollowedPerson(IGlobe globe)
        {
            return globe.SectorNodes.SelectMany(x => x.Sector.ActorManager.Items)
                .Where(x => x.Person.Fraction == Fractions.Pilgrims)
                .Where(x => !x.Person.GetModule<ISurvivalModule>().IsDead)
                .FirstOrDefault()?.Person;
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