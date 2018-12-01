using System;
using System.Collections.Generic;

using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public interface ISector
    {
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
        /// Событие выстреливает, когда все наблюдаемые актёры покинули сектор.
        /// </summary>
        event EventHandler ActorExit;

        IMap Map { get; }

        /// <summary>
        /// Стартовые узлы.
        /// Набор узлов, где могут располагаться актёры игрока
        /// на начало прохождения сектора.
        /// </summary>
        IMapNode[] StartNodes { get; set; }

        Dictionary<IActor, IPatrolRoute> PatrolRoutes { get; }
    }
}