using System;
using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration.LocalityStructures
{
    /// <summary>
    /// Простейшая реализация городской структуры.
    /// Эти структуры без каких либо условий будут потреблять и производить ресурсы.
    /// </summary>
    public class BasicLocalityStructure : ILocalityStructure
    {
        public BasicLocalityStructure(
            string name,
            Dictionary<PopulationSpecializations, int> requiredPopulation,
            Dictionary<LocalityResource, int> requiredResources,
            Dictionary<LocalityResource, int> productResources
            )
        {
            RequiredPopulation = requiredPopulation ?? throw new ArgumentNullException(nameof(requiredPopulation));
            RequiredResources = requiredResources ?? throw new ArgumentNullException(nameof(requiredResources));
            ProductResources = productResources ?? throw new ArgumentNullException(nameof(productResources));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Требования структуры к работникам.
        /// </summary>
        public Dictionary<PopulationSpecializations, int> RequiredPopulation { get; }

        /// <summary>
        /// Требования структуры к ресурсам города.
        /// </summary>
        public Dictionary<LocalityResource, int> RequiredResources { get; }

        /// <summary>
        /// Ресурс, производимый структурой.
        /// </summary>
        public Dictionary<LocalityResource, int> ProductResources { get; }

        /// <summary>
        /// Наименование.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Специальное наименование.
        /// </summary>
        public string SpeciaName { get; set; }

        /// <summary>
        /// Стоимость содержания структуры каждую итерацию.
        /// Сейчас для всех одна единица денег.
        /// Позже нужно будет сделать вариативность.
        /// </summary>
        public int MaintenanceCost => 1;

        public UnderConstructionData UnderConstructionData { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
