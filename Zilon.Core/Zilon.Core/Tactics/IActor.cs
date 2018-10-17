using System;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
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
    public interface IActor: IAttackTarget, IPassMapBlocker
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
        /// <param name="targetNode"> Целевой узел карты. </param>
        void MoveToNode(IMapNode targetNode);

        /// <summary>
        /// Открытие контейнера актёром.
        /// </summary>
        /// <param name="container"> Целевой контейнер в секторе. </param>
        /// <param name="method"> Метод открытия контейнера. </param>
        void OpenContainer(IPropContainer container, IOpenContainerMethod method);

        /// <summary>
        /// Происходит, когда актёр переместился.
        /// </summary>
        event EventHandler Moved;

        /// <summary>
        /// Происходит, когда актёр открывает контейнер в секторе.
        /// </summary>
        event EventHandler<OpenContainerEventArgs> OpenedContainer;

        /// <summary>
        /// Приенение действия к указанной цели.
        /// </summary>
        /// <param name="target"> Цель действия. </param>
        /// <param name="tacticalAct"> Тактическое действие, совершаемое над целью. </param>
        void UseAct(IAttackTarget target, ITacticalAct tacticalAct);

        void UseProp(IProp usedProp);

        /// <summary>
        /// Происходит, когда актёр выполняет действие.
        /// </summary>
        event EventHandler<UsedActEventArgs> UsedAct;

        //TODO Избавиться от этого метода.
        /// <summary>
        /// Вызывается службой действий в случае успешной обороны.
        /// </summary>
        void ProcessDefence();

        /// <summary>
        /// Событие выстреливает, когда персонаж успешно отражает наступление.
        /// </summary>
        event EventHandler OnDefence;
    }
}