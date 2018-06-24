using System;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Интерфейс актёра.
    /// </summary>
    /// <remarks>
    /// Актёр - это персонаж в бою. Характеристики актёра основаны на характеристиках
    /// персонажа, которого этот актёр отыгрывает. Состояниеи характеристики актёра могут меняться.
    /// Актёр может умереть.
    /// </remarks>
    public interface IActor: IAttackTarget
    {
        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        IPerson Person { get; }

        /// <summary>
        /// Владелец актёра.
        /// </summary>
        /// <remarks>
        /// 1. Опредляет возможность управлять актёром.
        /// 2. Боты по этому полю определяют противников.
        /// Может быть человек или бот.
        /// Персонажи игрока могут быть под прямым и не прямым управлением.
        /// </remarks>
        IPlayer Owner { get; }

        /// <summary>
        /// Перемещение актёра в указанный узел карты.
        /// </summary>
        /// <param name="targetNode"></param>
        void MoveToNode(IMapNode targetNode);

        /// <summary>
        /// Текущий урон актёра.
        /// </summary>
        float Damage { get; }

        /// <summary>
        /// Текущая дальность атаки.
        /// </summary>
        int WeaponDistance { get; }

        /// <summary>
        /// Текущий запас хитпоинтов.
        /// </summary>
        float Hp { get; }

        /// <summary>
        /// Состояние актёра.
        /// </summary>
        bool IsDead { get; set; }

        /// <summary>
        /// Происходит, когда актёр переместился.
        /// </summary>
        event EventHandler OnMoved;

        /// <summary>
        /// Происходит, если актёр умирает.
        /// </summary>
        event EventHandler OnDead;
    }
}