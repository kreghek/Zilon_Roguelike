using System.Collections.Generic;
using Zilon.Core.Diseases;
using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Данные персонажа о заболеваемости.
    /// </summary>
    public interface IDiseaseModule : IPersonModule
    {
        /// <summary>
        /// Текущие болезни персонажа.
        /// </summary>
        IEnumerable<IDiseaseProcess> Diseases { get; }

        /// <summary>
        /// Инфицирование персонажа указанной болезнью.
        /// </summary>
        /// <param name="disease"> Болезнь, которой будет инфицирован персонаж. </param>
        void Infect(IDisease disease);

        /// <summary>
        /// Процесс, обратный инфецированию. Удаляет болезнь из персонажа.
        /// </summary>
        /// <param name="disease"> Целевая болезнь. </param>
        void RemoveDisease(IDisease disease);

        /// <summary>
        /// Обновление состояния модуля болезней.
        /// </summary>
        /// <param name="personEffects"> Ссылка на модуль эффектов персонажа. Болезни навешивают эффекты. </param>
        void Update(IEffectsModule personEffects);
    }
}