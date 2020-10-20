using Zilon.Core.Tactics;

namespace Zilon.Core.Scoring
{
    /// <summary>
    /// Сервис для логирования событий, связанных с персонажем игрока
    /// </summary>
    public interface IPlayerEventLogService
    {
        /// <summary>
        /// Зафиксировать событие.
        /// </summary>
        /// <param name="playerEvent"></param>
        void Log(IPlayerEvent playerEvent);

        /// <summary>
        /// Актёр игрока.
        /// </summary>
        IActor Actor { get; set; }

        /// <summary>
        /// Вернуть последнее зарегистрированние событие.
        /// </summary>
        /// <returns></returns>
        IPlayerEvent GetPlayerEvent();
    }
}