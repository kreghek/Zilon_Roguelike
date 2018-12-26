using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    public interface IMonsterGeneratorRandomSource
    {
        int RollRarity();
        IMonsterScheme RollMonsterScheme(IEnumerable<IMonsterScheme> availableMonsterSchemes);
        int RollCount();
        int RollNodeIndex(int count);
    }
}
