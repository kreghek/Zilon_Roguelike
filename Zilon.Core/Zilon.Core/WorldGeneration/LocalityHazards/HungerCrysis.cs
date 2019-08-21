using System;

namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    /// <summary>
    /// Кризис голода.
    /// Если в городе еды меньше, чем требуется населению, то за каждую недостающую единицу
    /// еды с вероятностью 7+ при 2D6 умрёт случайная единица населения.
    /// </summary>
    public sealed class HungerCrysis : ICrysis
    {
        private readonly ICrysisRandomSource _crysisRandomSource;

        public HungerCrysis(ICrysisRandomSource crysisRandomSource)
        {
            _crysisRandomSource = crysisRandomSource;
        }

        public bool Update(Locality locality)
        {
            var currentFood = locality.Stats.Resources[LocalityResource.Food];
            if (currentFood >= 0)
            {
                return false;
            }

            // Голод забирает столько населения, сколько пищи не хватает.
            var populationDown = Math.Abs(currentFood);
            for (var i = 0; i < populationDown; i++)
            {
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
