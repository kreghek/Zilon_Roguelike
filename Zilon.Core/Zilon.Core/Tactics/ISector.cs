using System;
using System.Collections.Generic;

using Zilon.Core.Diseases;
using Zilon.Core.MapGenerators;
using Zilon.Core.Schemes;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Сектор (игровая локация). Используется в тактическом режиме.
    /// </summary>
    public interface ISector
    {
        IActorManager ActorManager { get; }

        /// <summary>
        /// Текущие болезни в секторе.
        /// </summary>
        /// <remarks>
        /// Если в секторе есть болезни, то один из монстров будет инфицирован этой болезнью.
        /// </remarks>
        IEnumerable<IDisease> Diseases { get; }

        /// <summary>
        /// Карта в основе сектора.
        /// </summary>
        ISectorMap Map { get; }

        /// <summary>
        /// Маршруты патрулирования в секторе.
        /// </summary>
        Dictionary<IActor, IPatrolRoute> PatrolRoutes { get; }

        ILocationScheme Scheme { get; set; }

        /// <summary>Менеджер работы с очками.</summary>
        IScoreManager ScoreManager { get; set; }

        IStaticObjectManager StaticObjectManager { get; }

        void AddDisease(IDisease disease);

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
        /// Вызывает актёр, когда хочет переёти из одного сектора в другой.
        /// </summary>
        /// <param name="actor"> Актёр, применивший переход. </param>
        /// <param name="transition"> переход, который бы задействован. </param>
        void UseTransition(IActor actor, RoomTransition transition);

        /// <summary>
        /// Событие выстреливает, когда группа актёров игрока покинула сектор.
        /// </summary>
        event EventHandler<TransitionUsedEventArgs> TrasitionUsed;
    }
}