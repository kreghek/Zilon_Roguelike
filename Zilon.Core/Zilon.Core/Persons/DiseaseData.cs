using System.Collections.Generic;

namespace Zilon.Core.Persons
{
    public class DiseaseData : IDiseaseData
    {
        private readonly List<IDisease> _diseases;

        public DiseaseData()
        {
            _diseases = new List<IDisease>();
        }

        public IEnumerable<IDisease> Diseases { get => _diseases; }

        public void Infect(IDisease disease)
        {
            _diseases.Add(disease);
        }
    }
}
