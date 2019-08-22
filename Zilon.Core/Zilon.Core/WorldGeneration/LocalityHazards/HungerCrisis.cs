using System;
using System.Linq;

namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    /// <summary>
    /// Кризис голода.
    /// Если в городе еды меньше, чем требуется населению, то за каждую недостающую единицу
    /// еды с вероятностью 7+ при 2D6 умрёт случайная единица населения.
    /// </summary>
    public sealed class HungerCrisis : ICrisis
    {
        private readonly ICrisisRandomSource _crysisRandomSource;

        public HungerCrisis(ICrisisRandomSource crysisRandomSource)
        {
            _crysisRandomSource = crysisRandomSource;
        }

        public bool Update(Locality locality)
        {
            // Если в городе нет населения, то не может быть и голода.
            // Сразу указываем, что этот кризис кончился.
            if (!locality.CurrentPopulation.Any())
            {
                return false;
            }

            var currentFood = locality.Stats.Resources[LocalityResource.Food];
            if (currentFood >= 0)
            {
                return false;
            }

            // Голод забирает столько населения, сколько пищи не хватает.
            var populationDown = Math.Abs(currentFood);
            for (var i = 0; i < populationDown; i++)
            {
                // Если в городе нет населения, то не может быть и голода.
                // Сразу указываем, что этот кризис кончился.
                // В этом случае голод не успел разрешиться, как всё население уже вымерло.
                // То есть голод был очень сильным (или население очень слабым).
                if (!locality.CurrentPopulation.Any())
                {
                    return false;
                }

                var deadPopulationRoll = _crysisRandomSource.RollDeathPass();
                var deathPassSuccess = GetDeathPass();
                if (deadPopulationRoll < deathPassSuccess)
                {
                    var deadPopulationIndex = _crysisRandomSource.RollDeadPopulationIndex(locality.CurrentPopulation);
                    locality.CurrentPopulation.RemoveAt(deadPopulationIndex);
                }
            }

            return true;
        }

        private int GetDeathPass()
        {
            // TODO В зависимости от города и населения это значение будет изменяться.
            return 7;
        }
    }
}
