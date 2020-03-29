using System.Collections.Generic;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Данные персонажа о заболеваемости.
    /// </summary>
    public interface IDiseaseData
    {
        /// <summary>
        /// Текущие болезни персонажа.
        /// </summary>
        IEnumerable<IDisease> Diseases { get; }

        /// <summary>
        /// Инфицирование персонажа указанной болезнью.
        /// </summary>
        /// <param name="disease"> Болезнь, которой будет инфицирован персонаж. </param>
        void Infect(IDisease disease);
    }
}
