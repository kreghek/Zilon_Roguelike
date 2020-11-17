using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Модуль болезней для монстров.
    /// Значительно упрошен по сравнению с базовой реализацией.
    /// Не рассчитывается прогресс болезни. Фактически, заболевший монстр не получает штрафов и болеет всю жизнь.
    /// Пока игрок не вырежет его.
    /// </summary>
    public class MonsterDiseaseModule : DiseaseModuleBase
    {
        protected override void UpdateDeseaseProcess(IDiseaseProcess diseaseProcess)
        {
            // Do nothing.
            // Disease progress is not calculated.
            // In fact, the sick monster does not receive any penalties and is sick for life.
        }
    }
}