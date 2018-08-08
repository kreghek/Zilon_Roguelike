using System;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class PersonFactory : IPersonFactory
    {
        public IPerson CreateHumanPerson(PersonScheme scheme)
        {
            throw new NotImplementedException();
        }

        public IPerson CreateMonsterPerson(MonsterScheme scheme, int level)
        {
            throw new NotImplementedException();
        }
    }
}
