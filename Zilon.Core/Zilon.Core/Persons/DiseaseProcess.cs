using System;

using Zilon.Core.Diseases;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Объект для хранения информации о протекании болезни в персонаже.
    /// </summary>
    public class DiseaseProcess : IDiseaseProcess
    {
        private const float DISEASE_SPEED = 0.005f;

        public DiseaseProcess(IDisease disease)
        {
            Disease = disease ?? throw new ArgumentNullException(nameof(disease));
        }

        /// <summary>
        /// Болезнь, которой инфицирован персонаж.
        /// </summary>
        public IDisease Disease { get; }

        /// <summary>
        /// Текущий прогресс по болезни. Принимает значение 0..1.
        /// При 0 - никак себя не проявляет.
        /// При 0.5 - обычно пик болезни.
        /// При 1 - болезнь ушла.
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Сила болезни.
        /// От силы зависит:
        /// влияние болезни,
        /// эффекты болезни (симптомы).
        /// </summary>
        public float CurrentPower { get => (float)Math.Sin(Value * 2 * Math.PI); }

        public void Update()
        {
            Value += DISEASE_SPEED;
        }
    }
}
