namespace Zilon.Core.StaticObjectModules
{
    public interface IStaticObjectModule
    {
        /// <summary>
        /// Признак активности модуля.
        /// Если модуль не активен, можно считать, что его нет.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Уникальный ключ модуля.
        /// Используется для хранения и извлечения модуля.
        /// </summary>
        string Key { get; }
    }
}