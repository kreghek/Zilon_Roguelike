using Zilon.Core.Players;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Настройки генерации монстров при создании сектора.
    /// </summary>
    public interface IMonsterGeneratorOptions
    {
        /// <summary>
        /// Схемы обычных монстров.
        /// </summary>
        string[] RegularMonsterSids { get; set; }

        /// <summary>
        /// Схемы редких монстров.
        /// Сейчас их 10 штук на сектор.
        /// </summary>
        string[] RareMonsterSids { get; set; }

        /// <summary>
        /// Схемы возможных чемпионов в секторе. Это боссы или уникальные монстры.
        /// Он 1 на сектор.
        /// </summary>
        string[] ChampionMonsterSids { get; set; }

        /// <summary>
        /// Ссылка на объект игрока, который будет управлять монстрами.
        /// По сути, маркер команды монстров.
        /// </summary>
        IBotPlayer BotPlayer { get; set; }


    }
}
