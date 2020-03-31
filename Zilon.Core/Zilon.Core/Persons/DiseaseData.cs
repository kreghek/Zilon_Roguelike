using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Diseases;

namespace Zilon.Core.Persons
{
    public class DiseaseData : IDiseaseData
    {
        private readonly List<IDiseaseProcess> _diseases;

        public DiseaseData()
        {
            _diseases = new List<IDiseaseProcess>();
        }

        public IEnumerable<IDiseaseProcess> Diseases { get => _diseases; }

        public void Infect(IDisease disease)
        {
            var currentProcess = _diseases.SingleOrDefault(x => x.Disease == disease);

            if (currentProcess is null)
            {
                currentProcess = new DiseaseProcess(disease);
                _diseases.Add(currentProcess);
            }
        }

        public void RemoveDisease(IDisease disease)
        {
            var currentProcess = _diseases.SingleOrDefault(x => x.Disease == disease);
            _diseases.Remove(currentProcess);
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}
