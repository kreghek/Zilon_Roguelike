﻿using System.Collections.Generic;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Модуль для работы с атрибутами персонажа: сила, ловкость, интеллект.
    /// </summary>
    public interface IAttributesModule : IPersonModule
    {
        /// <summary>
        /// Возвращает значение конкретного атрибута персонажа.
        /// </summary>
        /// <param name="personAttributeType"> Требуемый тип атриубта. </param>
        /// <returns> Возвращает атрибут. </returns>
        PersonAttribute GetAttribute(PersonAttributeType personAttributeType);

        /// <summary>
        /// Возвращает перечень всех атрибутов персонажа.
        /// </summary>
        /// <returns></returns>
        IEnumerable<PersonAttribute> GetAttributes();
    }
}