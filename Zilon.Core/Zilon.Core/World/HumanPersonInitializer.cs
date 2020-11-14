using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;
using Zilon.Core.Players;

namespace Zilon.Core.World
{
    public sealed class HumanPersonInitializer : IPersonInitializer
    {
        private const string PERSON_SCHEME_SID = "human-person";

        private readonly IPersonFactory _personFactory;
        private readonly IPlayer _player;

        public HumanPersonInitializer(IPersonFactory personFactory, IPlayer player)
        {
            _personFactory = personFactory ?? throw new ArgumentNullException(nameof(personFactory));
            _player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public Task<IEnumerable<IPerson>> CreateStartPersonsAsync(IGlobe globe)
        {
            var person = CreateStartPerson(PERSON_SCHEME_SID, _personFactory, Fractions.MainPersonFraction);
            _player.BindPerson(globe, person);
            return Task.FromResult(new[]
            {
                person
            }.AsEnumerable());
        }

        /// <summary>
        /// Создаёт персонажа.
        /// </summary>
        /// <returns> Возвращает созданного персонажа. </returns>
        private static IPerson CreateStartPerson(
            string personSchemeSid,
            IPersonFactory personFactory,
            IFraction fraction)
        {
            var startPerson = personFactory.Create(personSchemeSid, fraction);
            return startPerson;
        }
    }
}