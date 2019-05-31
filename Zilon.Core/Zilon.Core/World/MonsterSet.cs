using Zilon.Core.Schemes;

namespace Zilon.Core.World
{
    /// <summary>
    /// Набор монстров для генерации в узлах провинции.
    /// </summary>
    public sealed class MonsterSet
    {
        public IMonsterScheme[] MonsterSchemes { get; set; }
    }
}
