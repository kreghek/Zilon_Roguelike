using Zilon.Core.Players;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс персонажа.
    /// </summary>
    /// <remarks>
    /// Персонаж - это описание игрового объекта за пределами тактических боёв.
    /// </remarks>
    public interface IPerson
    {
        int Id { get; set; }

        /// <summary>
        /// Урон персонажа.
        /// </summary>
        float Damage { get; }

        /// <summary>
        /// Хитпоинты персонажа.
        /// </summary>
        float Hp { get; }

        /// <summary>
        /// Игрок персонажа.
        /// </summary>
        /// <remarks>
        /// Может быть человек или бот.
        /// Персонажи игрока могут быть под прямым и не прямым управлением.
        /// </remarks>
        //TODO Игрока пернести в актёра.
        IPlayer Player { get; }
    }
}