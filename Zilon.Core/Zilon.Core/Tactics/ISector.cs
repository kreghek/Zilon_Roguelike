using System;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public interface ISector
    {
        /// <summary>
        /// Точки выхода из сектора.
        /// </summary>
        IMapNode[] ExitNodes { get; set; }

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
    }
}