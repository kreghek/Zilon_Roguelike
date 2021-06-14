using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonGeneration
{
    public interface IMonsterPersonFactory
    {
        IPerson Create(IMonsterScheme monsterScheme);
    }
}