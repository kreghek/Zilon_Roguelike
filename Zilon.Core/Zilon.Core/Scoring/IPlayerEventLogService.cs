using Zilon.Core.Persons;
using Zilon.Core.Players;

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

        IPlayer Player { get; }

        /// <summary>
        /// Вернуть последнее зарегистрированние событие.
        /// </summary>
        /// <returns></returns>
        IPlayerEvent GetPlayerEvent();
    }
}