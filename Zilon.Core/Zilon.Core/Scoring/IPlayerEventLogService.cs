using Zilon.Core.Players;

namespace Zilon.Core.Scoring
{
    /// <summary>
    /// Сервис для логирования событий, связанных с персонажем игрока
    /// </summary>
    public interface IPlayerEventLogService
    {
        IPlayer Player { get; }

        /// <summary>
        /// Вернуть последнее зарегистрированние событие.
        /// </summary>
        /// <returns></returns>
        IPlayerEvent GetPlayerEvent();

        /// <summary>
        /// Зафиксировать событие.
        /// </summary>
        /// <param name="playerEvent"></param>
        void Log(IPlayerEvent playerEvent);
    }
}