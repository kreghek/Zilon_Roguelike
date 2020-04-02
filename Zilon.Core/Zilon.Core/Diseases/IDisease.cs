﻿namespace Zilon.Core.Diseases
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
        DiseaseSymptom[] GetSymptoms();

        /// <summary>
        /// Скорость протекания болезни.
        /// </summary>
        float ProgressSpeed { get; }
    }
}
