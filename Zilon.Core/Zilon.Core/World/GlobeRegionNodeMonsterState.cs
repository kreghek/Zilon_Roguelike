using Zilon.Core.Persons;

namespace Zilon.Core.World
{
    /// <summary>
    /// Текущее состояние монстров в узле  на глобальной карте.
    /// </summary>
    public sealed class GlobeRegionNodeMonsterState
    {
        /// <summary>
        /// Текущие монстры в узле провинции на глобальной карте.
        /// </summary>
        public MonsterPerson[] MonsterPersons { get; set; }
    }
}
