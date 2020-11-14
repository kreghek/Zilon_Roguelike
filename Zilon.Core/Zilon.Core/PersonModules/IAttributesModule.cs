namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Модуль для работы с атрибутами персонажа: сила, ловкость, интеллект.
    /// </summary>
    public interface IAttributesModule : IPersonModule
    {
        /// <summary>
        /// Возвращает перечень всех атрибутов персонажа.
        /// </summary>
        /// <returns></returns>
        IEnumerable<PersonAttribute> GetAttributes();

        /// <summary>
        /// Возвращает значение конкретного атрибута персонажа.
        /// </summary>
        /// <param name="personAttributeType"> Требуемый тип атриубта. </param>
        /// <returns> Возвращает атрибут. </returns>
        PersonAttribute GetAttribute(PersonAttributeType personAttributeType);
    }
}