using System;
using System.Collections.Generic;

using Zilon.Core.Components;
using Zilon.Core.Diseases;

namespace Zilon.Core.Persons.Survival
{
    public class DiseaseSymptomEffect : IPersonEffect
    {
        /// <summary>
        /// Болезни, которые провоцируют этот симптом.
        /// </summary>
        private readonly List<IDisease> _diseases;

        public DiseaseSymptomEffect(IDisease disease, DiseaseSymptom symptom)
        {
            if (disease is null)
            {
                throw new ArgumentNullException(nameof(disease));
            }

            Symptom = symptom ?? throw new ArgumentNullException(nameof(symptom));

            _diseases = new List<IDisease>();

            AddDisease(disease);
        }

        public IList<IDisease> Diseases { get => _diseases; }

        public DiseaseSymptom Symptom { get; }

        public void AddDisease(IDisease disease)
        {
            _diseases.Add(disease);

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveDisease(IDisease disease)
        {
            _diseases.Remove(disease);

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Changed;

        public EffectRule[] GetRules()
        {
            return new[] { new EffectRule(RollEffectType.Efficient, PersonRuleLevel.Lesser) };
        }
    }
}
