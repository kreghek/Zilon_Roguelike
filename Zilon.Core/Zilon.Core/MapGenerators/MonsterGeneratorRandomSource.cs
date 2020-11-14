using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.MapGenerators
{
    public class MonsterGeneratorRandomSource : IMonsterGeneratorRandomSource
    {
        private readonly IDice _dice;

        public MonsterGeneratorRandomSource(IDice dice)
        {
            _dice = dice ?? throw new ArgumentNullException(nameof(dice));
        }

        public IEnumerable<IPerson> RollInfectedMonsters(IEnumerable<IPerson> monsters, float diseasePower)
        {
            if (!monsters.Any())
            {
                return Array.Empty<IPerson>();
            }

            var currentMonsterCount = monsters.Count();
            var infectedCount = (int)Math.Ceiling(currentMonsterCount * diseasePower);

            var openList = new List<IPerson>(monsters);
            var resultList = new List<IPerson>();
            for (var i = 0; i < infectedCount; i++)
            {
                var rolledMonster = _dice.RollFromList(openList);
                resultList.Add(rolledMonster);
                openList.Remove(rolledMonster);
            }

            return resultList;
        }

        /// <summary>Выбирает случайную схему монстра среди доступных.</summary>
        /// <param name="availableMonsterSchemes">
        ///     Доступные схемы монстров.
        ///     Расчитываются исходя из схемы сектора и выбранной редкости.
        /// </param>
        /// <returns>Возвращает схему монстра.</returns>
        public IMonsterScheme RollMonsterScheme(IEnumerable<IMonsterScheme> availableMonsterSchemes)
        {
            var count = availableMonsterSchemes.Count();
            var rollIndex = _dice.Roll(0, count - 1);
            return availableMonsterSchemes.ElementAt(rollIndex);
        }

        /// <summary>
        ///     Выбирает слуяайный индекс узла, в который будет размещён монстр.
        /// </summary>
        /// <param name="count">Количество узлов в коллекции доступных узлов.</param>
        /// <returns>Индекс узла карты.</returns>
        public int RollNodeIndex(int count)
        {
            return _dice.Roll(0, count - 1);
        }

        /// <summary>Выбирает случайную редкость моснтра.</summary>
        /// <returns>2 - чемпион, 1 - редкий, 0 - обычный.</returns>
        public int RollRarity()
        {
            return _dice.Roll(0, 2);
        }

        /// <summary>Выбирает случайное количество монстров в секторе.</summary>
        /// <param name="regionMinCount">Минимальное количество монстров в секторе.</param>
        /// <param name="regionMaxCount">Максимальное количество монстров в секторе.</param>
        /// <returns>
        ///     Возвращает слуяайное количество монстров в секторе от 0 до указанного максимального числа.
        /// </returns>
        public int RollRegionCount(int regionMinCount, int regionMaxCount)
        {
            return _dice.Roll(regionMinCount, regionMaxCount);
        }
    }
}