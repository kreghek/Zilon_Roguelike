using System.Threading.Tasks;

using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace Zilon.Emulation.Common
{
    /// <summary>
    /// The content implementation to follow specified person.
    /// </summary>
    public sealed class FollowSinglePersonAutoplayContext : IAutoplayContext
    {
        private readonly IPerson _followedPerson;

        public FollowSinglePersonAutoplayContext(IPerson followedPerson)
        {
            _followedPerson = followedPerson;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckNextIterationAsync()
        {
            return await Task.Run(() =>
            {
                return !_followedPerson.GetModule<ISurvivalModule>().IsDead;
            });
        }
    }
}