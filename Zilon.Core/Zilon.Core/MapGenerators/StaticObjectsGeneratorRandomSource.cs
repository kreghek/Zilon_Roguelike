using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    public sealed class StaticObjectsGeneratorRandomSource : IStaticObjectsGeneratorRandomSource
    {
        private readonly IDice _dice;

        public StaticObjectsGeneratorRandomSource(IDice dice)
        {
            _dice = dice;
        }

        public PropContainerPurpose RollPurpose(PropContainerPurpose[] purposes)
        {
            return _dice.RollFromList(purposes);
        }
    }
}
