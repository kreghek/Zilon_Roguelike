using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Diseases
{
    /// <summary>
    /// Объект, представляющий болезнь в игре.
    /// </summary>
    public class Disease : IDisease
    {
        private readonly DiseaseSymptom[] _symptoms;

        /// <inheritdoc/>
        public DiseaseName Name { get; }

        /// <inheritdoc/>
        public DiseaseSymptom[] GetSymptoms() { return _symptoms; }

        public Disease(DiseaseName name, IEnumerable<DiseaseSymptom> symptoms)
        {
            if (symptoms is null)
            {
                throw new System.ArgumentNullException(nameof(symptoms));
            }

            Name = name;
            _symptoms = symptoms.ToArray();
        }
    }
}
