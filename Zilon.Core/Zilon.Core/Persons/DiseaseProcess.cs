using Zilon.Core.Diseases;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Объект для хранения информации о протекании болезни в персонаже.
    /// </summary>
    public class DiseaseProcess : IDiseaseProcess
    {
        public DiseaseProcess(IDisease disease)
        {
            Disease = disease ?? throw new ArgumentNullException(nameof(disease));
        }

        private static float CalcPowerByProgress(float progress)
        {
            var power = Math.Sin(progress * Math.PI);

            if (power < 0)
            {
                return 0;
            }

            if (power > 1)
            {
                return 1;
            }

            return (float)power;
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
        public float CurrentPower => CalcPowerByProgress(Value);

        public void Update()
        {
            Value += Disease.ProgressSpeed;
        }
    }
}