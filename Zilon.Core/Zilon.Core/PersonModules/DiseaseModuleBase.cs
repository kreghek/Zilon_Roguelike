using System.Collections.Generic;

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
        private readonly ICollection<IDiseaseProcess> _diseasesList;

        protected DiseaseModuleBase()
        {
            _diseasesList = new List<IDiseaseProcess>();
            IsActive = true;
        }

        protected abstract void UpdateDeseaseProcess(IEffectsModule personEffects, IDiseaseProcess diseaseProcess);

        public string Key => nameof(IDiseaseModule);

        /// <inheritdoc />
        public bool IsActive { get; set; }

        public IEnumerable<IDiseaseProcess> Diseases { get; }

        public void Infect(IDisease disease)
        {
            var currentProcess = _diseasesList.SingleOrDefault(x => x.Disease == disease);

            if (currentProcess is null)
            {
                currentProcess = new DiseaseProcess(disease);
                _diseasesList.Add(currentProcess);
            }
        }

        public void RemoveDisease(IDisease disease)
        {
            var currentProcess = _diseasesList.SingleOrDefault(x => x.Disease == disease);
            _diseasesList.Remove(currentProcess);
        }

        public void Update(IEffectsModule personEffects)
        {
            throw new System.NotImplementedException();
        }
    }
}