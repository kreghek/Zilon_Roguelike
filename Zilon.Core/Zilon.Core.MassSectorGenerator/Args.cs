namespace Zilon.Core.MassSectorGenerator
{
    /// <summary>
    /// Аргументы, используемые в текущем консольном приложении.
    /// </summary>
    internal static class Args
    {
        /// <summary>
        /// Зерно генерации.
        /// Ипользуется для того, чтобы получать одинаковые результаты при одном зерне генерации.
        /// Если значение не задано, то будет использовано случайное зерно генерации.
        /// </summary>
        public const string DICE_SEED_ARG_NAME = "dice_seed";

        /// <summary>
        /// Sid локации из каталога схем.
        /// Если это значение не задано, то будет выбрана случайная локация.
        /// </summary>
        public const string LOCATION_SCHEME_SID_ARG_NAME = "location";

        /// <summary>
        /// Sid сектора в указанной локации.
        /// Если это значение не задано, то будет выбран случайный сектор в выбранной локации.
        /// Помним, что локация может быть выбрана случайно.
        /// </summary>
        public const string SECTOR_SCHEME_SID_ARG_NAME = "sector";

        /// <summary>
        /// Полный путь и наименование выходного файла.
        /// Выходной файл будет в формате bmp.
        /// </summary>
        public const string OUT_PATH_ARG_NAME = "out";
    }
}