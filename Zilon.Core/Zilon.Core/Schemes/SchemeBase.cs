namespace Zilon.Core.Schemes
{
    /// <inheritdoc />
    /// <summary>
    /// Базовая схема с базовой реализацией.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class SchemeBase : IScheme
    {
        /// <summary>
        /// Строковое представление рецепта.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Sid}: {Name}";
        }

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
        [JsonProperty]
        public virtual LocalizedStringSubScheme Name { get; protected set; }

        /// <inheritdoc />
        /// <summary>
        /// Описание схемы.
        /// </summary>
        /// <remarks>
        /// Абстрактное описание. Для конкретных схем используется по-разному.
        /// </remarks>
        [JsonProperty]
        public virtual LocalizedStringSubScheme Description { get; protected set; }

        /// <inheritdoc />
        /// <summary>
        /// Признак отключенной схемы.
        /// </summary>
        /// <remarks>
        /// Если установлен, то схема игнорируется службой схем
        /// </remarks>
        [JsonProperty]
        public virtual bool Disabled { get; protected set; }
    }
}