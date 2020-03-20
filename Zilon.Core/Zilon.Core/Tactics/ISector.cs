using System;
using System.Collections.Generic;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Сектор (игровая локация). Используется в тактическом режиме.
    /// </summary>
    /// <remarks>
    /// Сектор - это совокупность карты, персонажей, сундуков и других объектов
    /// и правила их взаимодействия.
    /// </remarks>
    public interface ISector
    {
        /// <summary>
        /// Карта в основе сектора.
        /// </summary>
        ISectorMap Map { get; }

        /// <summary>
        /// Менеджер актёров в секторе.
        /// Содержит всех актёров в текущем секторе.
        /// </summary>
        IActorManager ActorManager { get; }

        /// <summary>
        /// Менеджер контейнеров в секторе.
        /// Содержит все контейнеры в текущем секторе.
        /// </summary>
        IPropContainerManager PropContainerManager { get; }

        /// <summary>
        /// Маршруты патрулирования в секторе.
        /// </summary>
        Dictionary<IActor, IPatrolRoute> PatrolRoutes { get; }

        /// <summary>Менеджер работы с очками.</summary>
        IScoreManager ScoreManager { get; set; }

        /// <summary>
        /// Схема, на основе которой был выстроен сектор.
        /// </summary>
        ILocationScheme Scheme { get; set; }

        /// <summary>
        /// Обновление состояние сектора.
        /// </summary>
        /// <remarks>
        /// Включает в себя обработку текущих источников задач.
        /// Выполнение задач актёров на один шаг.
        /// Определение и обработка состояния актёров.
        /// </remarks>
        void Update();

        /// <summary>
        /// Использование перехода в секторе.
        /// </summary>
        /// <param name="transition"> Переход, который следует использовать. </param>
        void UseTransition(ISectorTransition transition);

        /// <summary>
        /// Событие выстреливает, когда группа актёров игрока покинула сектор.
        /// </summary>
        event EventHandler<SectorExitEventArgs> HumanGroupExit;
    }
}