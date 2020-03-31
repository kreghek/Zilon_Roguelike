using System.Collections.Generic;

namespace Zilon.Core.Diseases
{
    /// <summary>
    /// Объект, представляющий болезнь в игре.
    /// </summary>
    public class Disease : IDisease
    {
        /// <inheritdoc/>
        public DiseaseName Name { get; }

        /// <inheritdoc/>
        public IEnumerable<DiseaseSymptom> Symptoms { get; }

        public Disease(DiseaseName name, IEnumerable<DiseaseSymptom> symptoms)
        {
            Name = name;
            Symptoms = symptoms ?? throw new System.ArgumentNullException(nameof(symptoms));
        }
    }
}
