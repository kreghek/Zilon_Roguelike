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
            UpdateFollowedPerson(globe);
            _globe = globe;
        }

        private void UpdateFollowedPerson(IGlobe globe)
        {
            _currentFollowedPerson = globe.SectorNodes.SelectMany(x => x.Sector.ActorManager.Items)
                .Where(x => x.Person.Fraction == Fractions.Pilgrims)
                .Where(x => !x.Person.GetModule<ISurvivalModule>().IsDead)
                .FirstOrDefault()?.Person;
        }

        public async Task<bool> CheckNextIterationAsync()
        {
            return await Task.Run(() =>
            {
                if (_currentFollowedPerson is null)
                {
                    UpdateFollowedPerson(_globe);
                }

                if (_currentFollowedPerson is null)
                {
                    return false;
                }

                return !_currentFollowedPerson.GetModule<ISurvivalModule>().IsDead;
            });
        }
    }
}