using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Diseases;
using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Base implementation of desease module.
    /// Can carry and remove desease.
    /// Updating desease is abstract.
    /// </summary>
    public abstract class DiseaseModuleBase : IDiseaseModule
    {
        private readonly ICollection<IDiseaseProcess> _diseaseList;

        protected DiseaseModuleBase()
        {
            _diseaseList = new List<IDiseaseProcess>();
            IsActive = true;
        }

        protected abstract void UpdateDeseaseProcess(IDiseaseProcess diseaseProcess);

        public string Key => nameof(IDiseaseModule);

        /// <inheritdoc />
        public bool IsActive { get; set; }

        public IEnumerable<IDiseaseProcess> Diseases => _diseaseList;

        public void Infect(IDisease disease)
        {
            var currentProcess = _diseaseList.SingleOrDefault(x => x.Disease == disease);

            if (currentProcess is null)
            {
                currentProcess = new DiseaseProcess(disease);
                _diseaseList.Add(currentProcess);
            }
        }

        public void RemoveDisease(IDisease disease)
        {
            var currentProcess = _diseaseList.SingleOrDefault(x => x.Disease == disease);
            _diseaseList.Remove(currentProcess);
        }

        public void Update()
        {
            var diseaseMaterialized = Diseases.ToArray();
            foreach (var diseaseProcess in diseaseMaterialized)
            {
                UpdateDeseaseProcess(diseaseProcess);
            }
        }
    }
}