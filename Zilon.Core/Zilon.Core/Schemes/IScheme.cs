namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Интерфейс всех схем.
    /// </summary>
    /// <remarks>
    /// Схема - это метаданные базовой сущности системы.
    /// Контент каждой реализации схемы должен находиться в отдельной директории в каталоге схем.
    /// </remarks>
    public interface IScheme
    {
        /// <summary>
        /// Символьный идентификатор схемы.
        /// </summary>
        /// <remarks>
        /// Символьный идентификатор схемы используются для ссылок на схему.
        /// Например, в БД или из контента других схем.
        /// </remarks>
        string Sid { get; set; }

        /// <summary>
        /// Признак отключенной схемы.
        /// </summary>
        /// <remarks>
        /// Если установлен, то схема игнорируется службой схем.
        /// </remarks>
        bool Disabled { get; }

        /// <summary>
        /// Наименование схемы.
        /// </summary>
        /// <remarks>
        /// В конкретных реализациях схем используется по-разному.
        /// </remarks>
        LocalizedStringSubScheme Name { get; }

        /// <summary>
        /// Описание схемы.
        /// </summary>
        /// <remarks>
        /// Абстрактное описание. Для конкретных схем используется по-разному.
        /// </remarks>
        LocalizedStringSubScheme Description { get; }
    }
}