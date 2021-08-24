using System;

using Zilon.Core.Graphs;
using Zilon.Core.MapGenerators;
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
    public interface IActor : IAttackTarget, IPassMapBlocker, IIdentifiableMapObject
    {
        /// <summary>
        /// Указывает, может ли актёр выполнять задачи.
        /// </summary>
        bool CanExecuteTasks { get; }

        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        IPerson Person { get; }

        IActorTaskSource<ISectorTaskSourceContext> TaskSource { get; }

        /// <summary>
        /// Форсированное перемещение актёра в указанный узел карты.
        /// </summary>
        /// <param name="targetNode"> Целевой узел карты. </param>
        void ForcedMoveToNode(IGraphNode targetNode);

        /// <summary>
        /// Добыча ресурса из залежей.
        /// </summary>
        /// <param name="deposit"> Целевые залежи. </param>
        /// <param name="method"> Метод добычи. </param>
        void MineDeposit(IStaticObject deposit, IMineDepositMethod method);

        /// <summary>
        /// Перемещение актёра в указанный узел карты.
        /// </summary>
        /// <param name="targetNode"> Целевой узел карты. </param>
        void MoveToNode(IGraphNode targetNode);

        void MoveToOtherSector(ISector sector, SectorTransition sectorTransition);

        /// <summary>
        /// Открытие контейнера актёром.
        /// </summary>
        /// <param name="container"> Целевой контейнер в секторе. </param>
        /// <param name="method"> Метод открытия контейнера. </param>
        void OpenContainer(IStaticObject container, IOpenContainerMethod method);

        /// <summary>
        /// Method-wrapper (mb temp) to raise event.
        /// Event are nesessary to run animation and sound in clients.
        /// </summary>
        void PerformTransfer();

        void SwitchTaskSource(IActorTaskSource<ISectorTaskSourceContext> actorTaskSource);

        /// <summary>
        /// Приенение действия к указанной цели.
        /// </summary>
        /// <param name="targetNode"> Узел карты, в которую прозошло действие. </param>
        /// <param name="tacticalAct"> Тактическое действие, совершаемое над целью. </param>
        void UseAct(IGraphNode targetNode, ICombatAct tacticalAct);

        void UseProp(IProp usedProp);

        /// <summary>
        /// Происходит, когда актёр переместился.
        /// </summary>
        event EventHandler<ActorMoveEventArgs>? Moved;

        /// <summary>
        /// Происходит, когда актёр открывает контейнер в секторе.
        /// </summary>
        event EventHandler<OpenContainerEventArgs>? OpenedContainer;

        /// <summary>
        /// Происходит, когда актёр выполняет добычу ресурса в секторе.
        /// </summary>
        event EventHandler<MineDepositEventArgs>? DepositMined;

        /// <summary>
        /// Происходит, когда актёр выполняет действие.
        /// </summary>
        event EventHandler<UsedActEventArgs>? UsedAct;

        /// <summary>
        /// Происходит, когда актёр получает урон.
        /// </summary>
        event EventHandler<DamageTakenEventArgs>? DamageTaken;

        /// <summary>
        /// Выстреливает, когда актёр использует предмет.
        /// </summary>
        event EventHandler<UsedPropEventArgs>? UsedProp;

        event EventHandler? PropTransferPerformed;
        event EventHandler? BeginTransitionToOtherSector;
    }
}