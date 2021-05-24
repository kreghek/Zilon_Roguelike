using System;
using System.Collections.Generic;

using Zilon.Core.Components;
using Zilon.Core.Diseases;

namespace Zilon.Core.Persons.Survival
{
    public class DiseaseSymptomCondition : IPersonCondition
    {
        /// <summary>
        /// Болезни, которые провоцируют этот симптом.
        /// </summary>
        private readonly List<IDisease> _diseases;

        public DiseaseSymptomCondition(IDisease disease, DiseaseSymptom symptom)
        {
            if (disease is null)
            {
                throw new ArgumentNullException(nameof(disease));
            }

            Symptom = symptom ?? throw new ArgumentNullException(nameof(symptom));

            _diseases = new List<IDisease>();

            HoldDisease(disease);
        }

        public IList<IDisease> Diseases => _diseases;

        public DiseaseSymptom Symptom { get; }

        public void HoldDisease(IDisease disease)
        {
            if (!_diseases.Contains(disease))
            {
                _diseases.Add(disease);
            }

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void ReleaseDisease(IDisease disease)
        {
            _diseases.Remove(disease);

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public ConditionRule[] GetRules()
        {
            return new[] { new ConditionRule(RollEffectType.Efficient, PersonRuleLevel.Lesser) };
        }

        public event EventHandler? Changed;
    }
}