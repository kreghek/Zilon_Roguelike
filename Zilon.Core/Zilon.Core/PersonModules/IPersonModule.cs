namespace Zilon.Core.PersonModules
{
    public interface IPersonModule
    {
        /// <summary>
        ///     Уникальный ключ модуля.
        ///     Используется для хранения и извлечения модуля.
        /// </summary>
        string Key { get; }

        /// <summary>
        ///     Признак активности модуля.
        ///     Если модуль не активен, можно считать, что его нет.
        /// </summary>
        bool IsActive { get; set; }
    }
}