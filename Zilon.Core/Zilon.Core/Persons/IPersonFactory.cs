using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public interface IPersonFactory
    {
        IPerson CreateHumanPerson(PersonScheme scheme);
        IPerson CreateMonsterPerson(MonsterScheme scheme, int level);
    }
}
