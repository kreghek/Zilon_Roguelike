using System;
using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Объект для хранения и передачи характеристик города.
    /// </summary>
    public sealed class LocalityStats
    {
        public LocalityStats()
        {
            ResourcesLastIteration = new Dictionary<LocalityResource, float>();
            ResourcesStorage = new Dictionary<LocalityResource, float>();
            FillResources();
        }

        /// <summary>
        /// Ресурсы, добытые в прошлой итерации мира.
        /// </summary>
        /// <remarks>
        /// Баланс применяется в начале итерации.
        /// 
        /// Далее, в зависимости от эффекта баланса, начинают работать здания и происходить
        /// различные городские события.
        /// 
        /// Положительный баланс пополняет запасы города.
        /// Отрицательный баланс снижает запасы и накладывает штрафы на население и работу построек.
        /// 
        /// Максимальное население в городе. Места проживания.
        /// Если фактическое население превышает максимальное население в городе,
        /// то начинается перенаселение. Жители начинают мигрировать.
        /// Вырастает вероятность возникновения антисанитарии, криминала и т.д.
        /// 
        /// Еда. Каждую итерацию одна единица населения будет требовать одной единицы пищи при подсчёте баланса.
        /// Если баланс еды отрицательный, то в городе начинается голод. Вероятность возникновения голода выше,
        /// если остаётся меньше запасов еды.
        /// 
        /// Энергия. Каждое здание требует единицу энергии. Вся вырабатываемая энергия распределяется между зданиями в городе.
        /// Если здание получет меньше энергии, чем требуется, то его вырботка снижается.
        /// Для жилых секторов нехватка энергии влияет по особому (пока не ясно как).
        /// 
        /// Обеспечение населения города товарами (не едой).
        /// Товары народного потребления требуются каждой единице населения при расчёте баланса.
        /// Если баланс товаров опускается ниже нуля, начинается дефицит.
        /// При дефиците население начинает мигрировать в другой город.
        /// В режиме приключений в городе не будут продавать определённые товары.
        /// </remarks>
        public Dictionary<LocalityResource, float> ResourcesLastIteration { get; }

        /// <summary>
        /// Хранимые запасы ресурсов.
        /// Сейчас ограничения на хранение ресурсов захардкожены. Далее будут изменяться городскими структурами.
        /// </summary>
        public Dictionary<LocalityResource, float> ResourcesStorage { get; }

        public void AddResource(LocalityResource resource, int count)
        {
            ResourcesLastIteration[resource] += count;
        }

        public float GetResource(LocalityResource resource)
        {
            return ResourcesLastIteration[resource];
        }

        public void RemoveResource(LocalityResource resource, int count)
        {
            if (!ResourcesLastIteration.ContainsKey(resource))
            {
                ResourcesLastIteration[resource] = 0;
            }

            ResourcesLastIteration[resource] -= count;
        }

        /// <summary>
        /// Этот метод выполняем для того, чтобы словарь всегда содержал все ресурсы.
        /// Чтобы не нужно было каждый раз проверять наличие ключа.
        /// </summary>
        private void FillResources()
        {
            var allAvailableResources = Enum.GetValues(typeof(LocalityResource))
                            .Cast<LocalityResource>()
                            .Where(x => x != LocalityResource.Undefined);
            foreach (var resource in allAvailableResources)
            {
                ResourcesLastIteration[resource] = 0;
                ResourcesStorage[resource] = 0;
            }
        }
    }
}
