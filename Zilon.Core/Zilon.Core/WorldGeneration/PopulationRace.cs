namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Расы населения.
    /// </summary>
    public enum PopulationRace
    {
        /// <summary>
        /// Не определено. Если это значение, значит, скорее всего, ошибка.
        /// Сейчас не предполагается, что население будет без расы.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Обычные человеки.
        /// </summary>
        Human,

        /// <summary>
        /// Гаарн.
        /// Орки+Скандинавы.
        /// </summary>
        Gaarn,

        /// <summary>
        /// Алеберт.
        /// Горцы+животные.
        /// </summary>
        Alebert,

        /// <summary>
        /// СанОст.
        /// Азия+бывшие рабы+искуственные люди.
        /// </summary>
        SunOst,

        /// <summary>
        /// Демоны.
        /// Цигане, наёмники в демонической оболочке.
        /// </summary>
        Deamon,

        /// <summary>
        /// Культисты жидкого пса.
        /// Африка+вуду+кибернетика.
        /// </summary>
        Cold
    }
}
