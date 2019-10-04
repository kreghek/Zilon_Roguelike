namespace Zilon.Core.Client.Windows
{
    /// <summary>
    /// Менеджер для работы с общеигровыми модалами.
    /// </summary>
    public interface ICommonModalManager
    {
        /// <summary>
        /// Выводит окно с подтверждением выхода из игры.
        /// </summary>
        void ShowQuitComfirmationModal();

        /// <summary>
        /// Выводит окно с подтверждением выхода из игры на титульный экран (главное меню).
        /// </summary>
        void ShowQuitTitleComfirmationModal();

        /// <summary>
        /// Показывает окно с игровым счётом.
        /// </summary>
        void ShowScoreModal();

        /// <summary>
        /// Показывает окно с Кногий Истории.
        /// </summary>
        void ShowHistoryBookModal();
    }
}
