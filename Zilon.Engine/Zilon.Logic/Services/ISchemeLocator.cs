namespace Zilon.Logic.Services
{
    /// <summary>
    /// Локатор схем, возвращающий все схемы в виде текста.
    /// </summary>
    public interface ISchemeLocator
    {
        /// <summary>
        /// Возвращает все схемы из указанной директории.
        /// </summary>
        /// <param name="directory">Наименование директории (папка в файловой системе или в ресурсах Unity).</param>
        /// <returns>Набор схем из директории в виде текста.</returns>
        SchemeFile[] GetAll(string directory);
    }
}
