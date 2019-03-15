using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Tactics.Spatial.PathFinding;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Базовая реализация карты.
    /// </summary>
    public abstract class MapBase : IMap
    {
        private readonly IDictionary<IMapNode, IList<IPassMapBlocker>> _nodeBlockers;

        /// <summary>
        /// Регионы карты.
        /// </summary>
        public IList<MapRegion> Regions { get; }

        /// <summary>
        /// Список узлов карты.
        /// </summary>
        public abstract IEnumerable<IMapNode> Nodes { get; }

        protected MapBase()
        {
            Regions = new List<MapRegion>();

            _nodeBlockers = new Dictionary<IMapNode, IList<IPassMapBlocker>>();
        }

        /// <summary>
        /// Проверяет, является ли данная ячейка доступной для текущего актёра.
        /// </summary>
        /// <param name="targetNode">Целевая ячейка.</param>
        /// <param name="actor">Проверяемый актёр.</param>
        /// <returns>
        /// true, если указанный узел проходим для актёра. Иначе - false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// targetNode
        /// or
        /// actor
        /// </exception>
        public virtual bool IsPositionAvailableFor(IMapNode targetNode, IActor actor)
        {
            if (targetNode == null)
            {
                throw new ArgumentNullException(nameof(targetNode));
            }

            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            if (!_nodeBlockers.TryGetValue(targetNode, out IList<IPassMapBlocker> blockers))
            {
                return true;
            }

            if (blockers.All(x => x == actor))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Указывает, что узел карты освобождён одним из блоков.
        /// </summary>
        /// <param name="node">Узел, который будет освобождён указанным блоком.</param>
        /// <param name="blocker">Блокер, который освобождает узел.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Попытка освободить узел {node}
        /// or
        /// Попытка освободить узел {node}, который не заблокирован блокировщиком {blocker}
        /// </exception>
        public void ReleaseNode(IMapNode node, IPassMapBlocker blocker)
        {
            if (!_nodeBlockers.TryGetValue(node, out IList<IPassMapBlocker> blockers))
            {
                throw new InvalidOperationException($"Попытка освободить узел {node}, который не заблокирован.");
            }

            if (!blockers.Contains(blocker))
            {
                throw new InvalidOperationException($"Попытка освободить узел {node}, который не заблокирован блокировщиком {blocker}.");
            }

            blockers.Remove(blocker);
        }

        /// <summary>
        /// Указывает, что узел карты занят блоком.
        /// </summary>
        /// <param name="node">Узел, который будет занят указанным блоком.</param>
        /// <param name="blocker">Блокер, который занимает узел.</param>
        public void HoldNode(IMapNode node, IPassMapBlocker blocker)
        {
            if (!_nodeBlockers.TryGetValue(node, out IList<IPassMapBlocker> blockers))
            {
                blockers = new List<IPassMapBlocker>(1);
                _nodeBlockers.Add(node, blockers);
            }

            blockers.Add(blocker);
        }

        /// <summary>
        /// Возвращает узлы, напрямую соединённые с указанным узлом.
        /// </summary>
        /// <param name="node">Опорный узел, относительно которого выбираются соседние узлы.</param>
        /// <returns>
        /// Возвращает набор соседних узлов.
        /// </returns>
        public abstract IEnumerable<IMapNode> GetNext(IMapNode node);

        /// <summary>
        /// Создаёт ребро между двумя узлами графа карты.
        /// </summary>
        /// <param name="node1">Узел графа карты.</param>
        /// <param name="node2">Узел графа карты.</param>
        public abstract void AddEdge(IMapNode node1, IMapNode node2);

        /// <summary>
        /// Удаляет ребро между двумя узлами графа карты.
        /// </summary>
        /// <param name="node1">Узел графа карты.</param>
        /// <param name="node2">Узел графа карты.</param>
        public abstract void RemoveEdge(IMapNode node1, IMapNode node2);

        /// <summary>
        /// Добавляет новый узел графа.
        /// </summary>
        /// <param name="node"></param>
        public abstract void AddNode(IMapNode node);

        /// <summary>
        /// Выполняет поиск пути к указанному узлу.
        /// </summary>
        /// <param name="start">Начальный узел поиска пути.</param>
        /// <param name="end">Целевой узел поиска пути.</param>
        /// <param name="context">Контекст поиска пути.</param>
        /// <param name="outputPath">В результате будет содержать набор узлов,
        /// представляющих путь из указанного узла в целевой.</param>
        /// <remarks>
        /// Передача списка для результатов сделана для оптимизации - не нужно каждый раз создавать список
        /// и выделять под него память в зависимости от найденного пути.
        /// </remarks>
        public void FindPath(IMapNode start, IMapNode end, PathFindingContext context, List<IMapNode> outputPath)
        {
            var startNode = start;
            var finishNode = end;

            var astar = new AStar(this, context, startNode, finishNode);
            var resultState = astar.Run();
            if (resultState == State.GoalFound)
            {
                var foundPath = astar.GetPath().Skip(1).ToArray();
                foreach (var pathNode in foundPath)
                {
                    outputPath.Add((HexNode)pathNode);
                }
            }
        }
    }
}
