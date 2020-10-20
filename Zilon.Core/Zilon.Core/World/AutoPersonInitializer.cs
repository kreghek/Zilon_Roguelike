using System.Collections.Generic;
using System.Threading.Tasks;

using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;

namespace Zilon.Core.World
{
    public sealed class AutoPersonInitializer : IPersonInitializer
    {
        private readonly IFraction _pilgrimFraction;
        private readonly IPersonFactory _personFactory;

        public AutoPersonInitializer(IPersonFactory personFactory)
        {
            _pilgrimFraction = new Fraction("Pilgrims");

            _personFactory = personFactory;
        }

        public IEnumerable<IPerson> CreateStartPersonsInner()
        {
            for (var i = 0; i < 40; i++)
            {
                yield return CreateStartPerson("human-person", _personFactory, _pilgrimFraction);
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
