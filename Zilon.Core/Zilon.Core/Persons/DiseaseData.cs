using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Diseases;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация моделя болезней персонажа.
    /// </summary>
    public class DiseaseData : IDiseaseData
    {
        private readonly List<IDiseaseProcess> _diseases;

        public DiseaseData()
        {
            _diseases = new List<IDiseaseProcess>();
        }

        /// <inheritdoc/>
        public IEnumerable<IDiseaseProcess> Diseases { get => _diseases; }

        /// <inheritdoc/>
        public void Infect(IDisease disease)
        {
            var currentProcess = _diseases.SingleOrDefault(x => x.Disease == disease);

            if (currentProcess is null)
            {
                currentProcess = new DiseaseProcess(disease);
                _diseases.Add(currentProcess);
            }
        }

        /// <inheritdoc/>
        public void RemoveDisease(IDisease disease)
        {
            var currentProcess = _diseases.SingleOrDefault(x => x.Disease == disease);
            _diseases.Remove(currentProcess);
        }
    }
}
