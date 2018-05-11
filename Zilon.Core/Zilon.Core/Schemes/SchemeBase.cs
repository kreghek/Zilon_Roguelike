namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Базовая схема с базовой реализацией.
    /// </summary>
    public abstract class SchemeBase : IScheme
    {
        /// <summary>
        /// Символьный идентификатор схемы.
        /// </summary>
        /// <remarks>
        /// Символьный идентификатор схемы используются для ссылок на схему.
        /// Например, в БД или из контента других схем.
        /// </remarks>
        public string Sid { get; set; }

        /// <summary>
        /// Признак отключенной схемы.
        /// </summary>
        /// <remarks>
        /// Если установлен, то схема игнорируется службой схем
        /// </remarks>
        public bool Disabled { get; set; }
    }
}
