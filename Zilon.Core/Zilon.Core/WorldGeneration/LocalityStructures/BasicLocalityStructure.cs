using System.Collections.Generic;

namespace Zilon.Core.WorldGeneration.LocalityStructures
{
    /// <summary>
    /// Простейшая реализация городской структуры.
    /// Эти структуры без каких либо условий будут потреблять и производить ресурсы.
    /// </summary>
    public class BasicLocalityStructure : ILocalityStructure
    {
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
        public string SpeciaName { get; }
    }
}
