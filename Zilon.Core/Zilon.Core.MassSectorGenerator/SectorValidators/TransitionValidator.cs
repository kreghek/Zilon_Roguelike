using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Tactics;

namespace Zilon.Core.MassSectorGenerator.SectorValidators
{
    /// <summary>
    /// Валидатор переходов из сектора.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance",
        "CA1812:Avoid uninstantiated internal classes",
        Justification = "Регистрируется в контейнере зависимостей через рефлексию.")]
    class TransitionValidator : ISectorValidator
    {
        public Task Validate(ISector sector, IServiceProvider scopeContainer)
        {
            return Task.Run(() =>
            {
                var transitions = sector.Map.Transitions.Values;

                // В секторе должны быть выходы.

                if (transitions.Any())
                {
                    var hasStartRegion = false;
                    var hasTransitionInregionsNodes = false;
                    foreach (var region in sector.Map.Regions)
                    {
                        if (region.IsStart)
                        {
                            hasStartRegion = true;
                        }

                        if ((region.ExitNodes?.Any()).GetValueOrDefault())
                        {
                            hasTransitionInregionsNodes = true;
                        }
                    }

                    if (!hasStartRegion)
                    {
                        // Хоть один регион должен быть отмечен, как стартовый.
                        // Чтобы клиенты знали, где размещать персонажа после генерации.
                        throw new SectorValidationException($"Не задан стартовый регион.");
                    }

                    if (!hasTransitionInregionsNodes)
                    {
                        // Переходы должны быть явно обозначены в регионах.
                        //TODO Рассмотреть вариант упрощения
                        // В секторе уже есть информация об узлах с переходами.
                        // Выглядит, как дублирование.
                        throw new SectorValidationException($"Не указан ни один регион с узламы перехода.");
                    }
                }
                else
                {
                    // Если в секторе нет переходов, то будет невозможно его покинуть.
                    throw new SectorValidationException("В секторе не найдены переходы.");
                }
            });
        }
    }
}
