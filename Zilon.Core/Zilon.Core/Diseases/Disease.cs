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

        public Disease(DiseaseName name, IEnumerable<DiseaseSymptom> symptoms, float progressSpeed)
        {
            if (symptoms is null)
            {
                throw new System.ArgumentNullException(nameof(symptoms));
            }

            if (progressSpeed <= 0)
            {
                throw new System.ArgumentException("Скорость протекания болезни должна быть больше 0",
                    nameof(progressSpeed));
            }

            Name = name;
            ProgressSpeed = progressSpeed;
            _symptoms = symptoms.ToArray();
        }

        /// <inheritdoc/>
        public DiseaseName Name { get; }

        /// <inheritdoc/>
        public DiseaseSymptom[] GetSymptoms() { return _symptoms; }

        /// <inheritdoc/>
        public float ProgressSpeed { get; }
    }
}