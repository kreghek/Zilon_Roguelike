using System;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;
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
        /// Открытие контейнера актёром.
        /// </summary>
        /// <param name="container"> Целевой контейнер в секторе. </param>
        /// <param name="method"> Метод открытия контейнера. </param>
        void OpenContainer(IPropContainer container, IOpenContainerMethod method);

        /// <summary>
        /// Текущий запас хитпоинтов.
        /// </summary>
        float Hp { get; }

        /// <summary>
        /// Состояние актёра.
        /// </summary>
        bool IsDead { get; set; }

        /// <summary>
        /// Текущие возможные действия актёра.
        /// </summary>
        ITacticalAct Acts { get; }

        /// <summary>
        /// Происходит, когда актёр переместился.
        /// </summary>
        event EventHandler OnMoved;

        /// <summary>
        /// Происходит, если актёр умирает.
        /// </summary>
        event EventHandler OnDead;

        /// <summary>
        /// Происходит, когда актёр открывает контейнер в секторе.
        /// </summary>
        event EventHandler<OpenContainerEventArgs> OpenedContainer;

        /// <summary>
        /// Инициатива актёра.
        /// </summary>
        float Initiative { get; }

        /// <summary>
        /// Инвентарь персонажа.
        /// </summary>
        /// <remarks>
        /// Для монстров равен null.
        /// </remarks>
        IInventory Inventory { get; }
    }
}