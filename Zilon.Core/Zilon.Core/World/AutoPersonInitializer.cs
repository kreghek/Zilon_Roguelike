﻿using System.Collections.Generic;
using System.Threading.Tasks;

using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;

namespace Zilon.Core.World
{
    public sealed class AutoPersonInitializer : IPersonInitializer
    {
        private const string PERSON_SCHEME_SID = "human-person";
        private readonly IPersonFactory _personFactory;

        public AutoPersonInitializer(IPersonFactory personFactory)
        {
            _personFactory = personFactory;
        }

        /// <summary>
        /// Создаёт персонажа.
        /// </summary>
        /// <returns> Возвращает созданного персонажа. </returns>
        private static IPerson CreateStartPerson(string personSchemeSid, IPersonFactory personFactory,
            IFraction fraction)
        {
            var startPerson = personFactory.Create(personSchemeSid, fraction);
            return startPerson;
        }

        private IEnumerable<IPerson> CreateStartPersonsInner()
        {
            for (var i = 0; i < 40; i++)
            {
                yield return CreateStartPerson(PERSON_SCHEME_SID, _personFactory, Fractions.Pilgrims);
            }
        }

        public Task<IEnumerable<IPerson>> CreateStartPersonsAsync(IGlobe globe)
        {
            return Task.FromResult(CreateStartPersonsInner());
        }
    }
}