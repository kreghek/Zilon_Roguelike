using System;

using Zilon.Core.Graphs;
using Zilon.Core.Persons;
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
    public interface IActor : IAttackTarget, IPassMapBlocker
    {
        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        IPerson Person { get; }

        IActorTaskSource<ISectorTaskSourceContext> TaskSource { get; }

        void SwitchTaskSource(IActorTaskSource<ISectorTaskSourceContext> actorTaskSource);

        /// <summary>
        /// Перемещение актёра в указанный узел карты.
        /// </summary>
        /// <param name="targetNode"> Целевой узел карты. </param>
        void MoveToNode(IGraphNode targetNode);

        /// <summary>
        /// Открытие контейнера актёром.
        /// </summary>
        /// <param name="container"> Целевой контейнер в секторе. </param>
        /// <param name="method"> Метод открытия контейнера. </param>
        void OpenContainer(IStaticObject container, IOpenContainerMethod method);

        /// <summary>
        /// Добыча ресурса из залежей.
        /// </summary>
        /// <param name="deposit"> Целевые залежи. </param>
        /// <param name="method"> Метод добычи. </param>
        void MineDeposit(IStaticObject deposit, IMineDepositMethod method);

        /// <summary>
        /// Происходит, когда актёр переместился.
        /// </summary>
        event EventHandler Moved;

        /// <summary>
        /// Происходит, когда актёр открывает контейнер в секторе.
        /// </summary>
        event EventHandler<OpenContainerEventArgs> OpenedContainer;

        /// <summary>
        /// Происходит, когда актёр выполняет добычу ресурса в секторе.
        /// </summary>
        event EventHandler<MineDepositEventArgs> DepositMined;

        /// <summary>
        /// Происходит, когда актёр выполняет действие.
        /// </summary>
        event EventHandler<UsedActEventArgs> UsedAct;

        /// <summary>
        /// Происходит, когда актёр получает урон.
        /// </summary>
        event EventHandler<DamageTakenEventArgs> DamageTaken;

        /// <summary>
        /// Выстреливает, когда актёр использует предмет.
        /// </summary>
        event EventHandler<UsedPropEventArgs> UsedProp;

        /// <summary>
        /// Приенение действия к указанной цели.
        /// </summary>
        /// <param name="target"> Цель действия. </param>
        /// <param name="tacticalAct"> Тактическое действие, совершаемое над целью. </param>
        void UseAct(IAttackTarget target, ITacticalAct tacticalAct);

        void UseProp(IProp usedProp);

        /// <summary>
        /// Указывает, может ли актёр выполнять задачи.
        /// </summary>
        bool CanExecuteTasks { get; }
    }
}