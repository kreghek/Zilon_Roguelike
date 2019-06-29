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
        /// Событие выстреливает, когда персонаж успешно отражает наступление.
        /// </summary>
        event EventHandler<DefenceEventArgs> OnDefence;

        /// <summary>
        /// Происходит, когда актёр выполняет действие.
        /// </summary>
        event EventHandler<UsedActEventArgs> UsedAct;

        /// <summary>
        /// Происходит, когда актёр получает урон.
        /// </summary>
        event EventHandler<DamageTakenEventArgs> DamageTaken;

        /// <summary>
        /// Происходит, когда актёр успешно использует броню.
        /// </summary>
        event EventHandler<ArmorEventArgs> OnArmorPassed;

        /// <summary>
        /// Атака указанного актёра.
        /// </summary>
        /// <param name="target">Целевой актёр.</param>
        /// <param name="usedTacticalActs">Используемые для атаки действия.</param>
        void AttackActor(IActor target, UsedTacticalActs usedTacticalActs, IUseActResolver useActResolver);

        /// <summary>
        /// Атака указанный статический объект (не актёра).
        /// </summary>
        /// <param name="target">Целевой объект.</param>
        /// <param name="usedTacticalActs">Используемые для атаки дейсвтия.</param>
        void AttackStaticObject(IAttackTarget target, UsedTacticalActs usedTacticalActs, IUseActResolver useActResolver);

        /// <summary>
        /// Лечение указанного актёра.
        /// </summary>
        /// <param name="target">Целевой актёр. Актёр может применить действие на себя.</param>
        /// <param name="usedTacticalActs">Применяемые действия.</param>
        void HealActor(IActor target, UsedTacticalActs usedTacticalActs, IUseActResolver useActResolver);

        /// <summary>
        /// Использование предмета из инвентаря на себя.
        /// </summary>
        /// <param name="usedProp">Используемый предмет.</param>
        void UseProp(IProp usedProp);

        /// <summary>
        /// Вызывается службой действий в случае успешной обороны.
        /// </summary>
        void ProcessDefence(PersonDefenceItem prefferedDefenceItem, int successToHitRoll, int factToHitRoll);

        /// <summary>
        /// Вызывается службой действий в случае успешного использования брони.
        /// </summary>
        void ProcessArmor(int armorRank, int successRoll, int factRoll);
    }
}