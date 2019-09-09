using System.Collections.Generic;
using System.Linq;

using Zilon.Core.WorldGeneration.LocalityHazards;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Город.
    /// </summary>
    public class Locality
    {
        public Locality()
        {
            Regions = new List<LocalityRegion>();
            Stats = new LocalityStats();
            CurrentPopulation = new List<PopulationUnit>();
            Crises = new List<ICrisis>();
        }

        public string Name { get; set; }

        public TerrainCell Cell { get; set; }

        public Realm Owner { get; set; }

        /// <summary>
        /// Текущее население города.
        /// Каждый объект в списке - это единица населения.
        /// Суммарно единиц популяции не должно быть больше, чем места для проживания.
        /// Иначе начнётся перенаселение. Тогда жители могут организовать миграционнуб группу и покинуть город.
        /// </summary>
        public List<PopulationUnit> CurrentPopulation { get; }

        public Dictionary<BranchType, int> Branches { get; set; }

        /// <summary>
        /// Текущие районы города.
        /// Каждый район занимает один узел в провинции.
        /// Каждый район сначала должен быть разработан.
        /// После разработки в районе можно возводить структуры.
        /// </summary>
        public List<LocalityRegion> Regions { get; }

        /// <summary>
        /// Текущее состояние в городе. Харатектиристики города.
        /// </summary>
        public LocalityStats Stats { get; private set; }

        /// <summary>
        /// Текущие кризисы в городе.
        /// </summary>
        public List<ICrisis> Crises { get; }

        /// <summary>
        /// Текущий управляющий в городе.
        /// </summary>
        public Agent Head { get; set; }

        public override string ToString()
        {
            return $"{Name} [{Owner}] ({Branches?.FirstOrDefault().Key})";
        }

        /// <summary>
        /// Обновление состояния города.
        /// </summary>
        public void Update()
        {
            UpdatePopulation();
            UpdateRegions();
        }

        private void UpdateRegions()
        {
            foreach (var region in Regions)
            {
                // Проверяем, хватает ли денег для этого района.
                var money = Stats.GetResource(LocalityResource.Money);
                if (money >= region.MaintenanceCost)
                {
                    Stats.RemoveResource(LocalityResource.Money, region.MaintenanceCost);
                }
                else
                {
                    // Не хватает денег на содержание этого района.
                    // Все его строения не работают.
                    continue;
                }

                // Для жилых мест отдельная логика.
                // Их потребляет только население, а производят структуры.
                // Поэтому зануляем перед обработкой структур города. Далее структуры выставят текущее значение.
                Stats.Resources[LocalityResource.LivingPlaces] = 0;

                var suppliedStructures = SupplyStructures(region.Structures);
                ProduceResources(suppliedStructures, Stats);

                // Реализация производства.
                // Избыточное производтство даёт деньги. Предполагается, что избыточное производство
                // народ пускает на улучшение благосостояния.
                RealizeManufacture();
            }
        }

        private void RealizeManufacture()
        {
            var manufacture = Stats.GetResource(LocalityResource.Manufacture);
            if (manufacture > 0)
            {
                Stats.RemoveResource(LocalityResource.Manufacture, manufacture);
                Stats.AddResource(LocalityResource.Money, manufacture);
            }
        }

        private void UpdatePopulation()
        {
            // Изымаем столько товаров и еды, сколько населения в городе.
            var populationCount = CurrentPopulation.Count();

            Stats.RemoveResource(LocalityResource.Food, populationCount);
            Stats.RemoveResource(LocalityResource.Goods, populationCount);

            // Рассчитываем рост населения
            UpdatePopulationGrowth();
        }

        private void UpdatePopulationGrowth()
        {
            foreach (var unit in CurrentPopulation)
            {
                unit.UpdateGrowth();
            }
        }

        private static void ProduceResources(List<ILocalityStructure> structures, LocalityStats stats)
        {
            // Все структуры, которые получили обеспечение, производят ресурс
            foreach (var structure in structures)
            {
                foreach (var productResource in structure.ProductResources)
                {
                    if (!stats.Resources.ContainsKey(productResource.Key))
                    {
                        stats.Resources[productResource.Key] = 0;
                    }

                    stats.Resources[productResource.Key] += productResource.Value;
                }
            }
        }

        private List<ILocalityStructure> SupplyStructures(List<ILocalityStructure> structures)
        {
            // Структуры, которые получили обеспечение.
            var suppliedStructures = new List<ILocalityStructure>();

            // Изымаем все ресурсы текущй структурой.
            // Струкруты, которые получили обеспечение, затем производят ресурсы.
            foreach (var structure in structures)
            {
                // Проверяем, хватает ли денег для содержанния этой структуры.
                var money = Stats.GetResource(LocalityResource.Money);
                if (money >= structure.MaintenanceCost)
                {
                    Stats.RemoveResource(LocalityResource.Money, structure.MaintenanceCost);
                }
                else
                {
                    // Не хватает денег на содержание этой структуры.
                    // Она ничего не производит в эту итерацию.
                    continue;
                }

                // Проверка наличия необходимых ресурсов.
                foreach (var requiredResource in structure.RequiredResources)
                {
                    var requiredResourceType = requiredResource.Key;
                    if (Stats.Resources.ContainsKey(requiredResourceType))
                    {
                        if (Stats.Resources[requiredResourceType] >= requiredResource.Value)
                        {
                            suppliedStructures.Add(structure);
                            Stats.Resources[requiredResourceType] -= requiredResource.Value;
                        }
                    }

                }
            }

            return suppliedStructures;
        }
    }
}
