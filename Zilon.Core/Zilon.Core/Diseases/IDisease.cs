using System.Collections.Generic;

namespace Zilon.Core.Diseases
{
    /// <summary>
    /// Интерфейс болезни, поразившей персонажа.
    /// </summary>
    public interface IDisease
    {
        /// <summary>
        /// Наименование болезни.
        /// </summary>
        DiseaseName Name { get; }

        /// <summary>
        /// Симптомы болезни.
        /// </summary>
        IEnumerable<DiseaseSymptom> Symptoms { get; }
    }
}
