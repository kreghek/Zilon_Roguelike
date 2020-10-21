using System.Collections.Generic;
using System.Threading.Tasks;

using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;

namespace Zilon.Core.World
{
    public sealed class AutoPersonInitializer : IPersonInitializer
    {
        private const string PERSON_SCHEME_SID = "human-person";
        private const string START_FACTION_NAME = "Pilgrims";

        private readonly IFraction _pilgrimFraction;
        private readonly IPersonFactory _personFactory;

        public AutoPersonInitializer(IPersonFactory personFactory)
        {
            _pilgrimFraction = new Fraction(START_FACTION_NAME);

            _personFactory = personFactory;
        }

        public IEnumerable<IPerson> CreateStartPersonsInner()
        {
            for (var i = 0; i < 40; i++)
            {
                yield return CreateStartPerson(PERSON_SCHEME_SID, _personFactory, _pilgrimFraction);
            }
        }

        /// <summary>
        /// Создаёт персонажа.
        /// </summary>
        /// <returns> Возвращает созданного персонажа. </returns>
        private static IPerson CreateStartPerson(string personSchemeSid, IPersonFactory personFactory, IFraction fraction)
        {
            var startPerson = personFactory.Create(personSchemeSid, fraction);
            return startPerson;
        }

        public Task<IEnumerable<IPerson>> CreateStartPersonsAsync()
        {
            return Task.FromResult(CreateStartPersonsInner());
        }
    }
}