namespace Zilon.Core.Schemes
{
    /// <inheritdoc />
    /// <summary>
    /// Базовая схема с базовой реализацией.
    /// </summary>
    public abstract class SchemeBase : IScheme
    {
        /// <inheritdoc />
        /// <summary>
        /// Символьный идентификатор схемы.
        /// </summary>
        /// <remarks>
        /// Символьный идентификатор схемы используются для ссылок на схему.
        /// Например, в БД или из контента других схем.
        /// </remarks>
        public string Sid { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Наименование схемы.
        /// </summary>
        /// <remarks>
        /// В конкретных реализациях схем используется по-разному.
        /// </remarks>
        public string Name { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Описание схемы.
        /// </summary>
        /// <remarks>
        /// Абстрактное описание. Для конкретных схем используется по-разному.
        /// </remarks>
        public string Description { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Признак отключенной схемы.
        /// </summary>
        /// <remarks>
        /// Если установлен, то схема игнорируется службой схем
        /// </remarks>
        public bool Disabled { get; set; }
    }
}
