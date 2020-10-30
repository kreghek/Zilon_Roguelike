using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public interface ISpawnPerk : IPerk
    {
        ITacticalActScheme TacticalAct { get; }
        IPersonScheme PersonScheme { get; }
    }
}