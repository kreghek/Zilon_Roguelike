using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;

namespace Zilon.Core.MapGenerators
{
    public class DiseaseGenerator : IDiseaseGenerator
    {
        private readonly IDice _dice;
        private int _counter;

        public DiseaseGenerator(IDice dice)
        {
            _dice = dice;
        }

        public IDisease Create()
        {
            var roll = _dice.RollD6();
            if (roll == 1)
            {

                _counter++;
                var disease = new Disease($"disease {_counter}");
                return disease;
            }
            else
            {
                return null;
            }
        }
    }
}
