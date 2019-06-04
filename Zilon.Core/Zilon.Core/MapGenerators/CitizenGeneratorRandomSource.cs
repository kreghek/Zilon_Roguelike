using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.MapGenerators
{
    public sealed class CitizenGeneratorRandomSource : ICitizenGeneratorRandomSource
    {
        private readonly IDice _dice;

        public CitizenGeneratorRandomSource(IDice dice)
        {
            _dice = dice;
        }

        /// <inheridoc />
        public int RollNodeIndex(int nodeCount)
        {
            var rolledIndex = _dice.Roll(0, nodeCount - 1);
            return rolledIndex;
        }
    }
}
