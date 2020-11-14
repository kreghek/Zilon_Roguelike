using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Diseases;
using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Модуль болезней для монстров.
    /// Значительно упрошен по сравнению с базовой реализацией.
    /// Не рассчитывается прогресс болезни. Фактически, заюолевший монстр не получает штрафов и болеет всю жизнь.
    /// Пока игрок не вырежет его.
    /// </summary>
    public class MonsterDiseaseModule : IDiseaseModule
    {
        private readonly List<IDiseaseProcess> _diseases;

        public MonsterDiseaseModule()
        {
            _diseases = new List<IDiseaseProcess>();
            IsActive = true;
        }

        /// <inheritdoc/>
        public IEnumerable<IDiseaseProcess> Diseases => _diseases;

        /// <inheritdoc/>
        public string Key => nameof(IDiseaseModule);

        /// <inheritdoc/>
        public bool IsActive { get; set; }

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

        public void Update(IEffectsModule personEffects)
        {
            // Не обновляем прогресс болзней монстров.
            // Монстры болеют всю жизнь и не страдают от штрафов.
        }
    }
}