using Zilon.Core.Graphs;
using Zilon.Core.PathFinding;
using Zilon.Core.Persons;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Базовая реализация карты.
    /// </summary>
    public abstract class MapBase : IMap
    {
        private readonly IDictionary<IGraphNode, IList<IPassMapBlocker>> _nodeBlockers;

        protected MapBase()
        {
            Regions = new List<MapRegion>();

            _nodeBlockers = new Dictionary<IGraphNode, IList<IPassMapBlocker>>();
        }

        /// <summary>
        /// Регионы карты.
        /// </summary>
        public IList<MapRegion> Regions { get; }

        /// <summary>
        /// Список узлов карты.
        /// </summary>
        public abstract IEnumerable<IGraphNode> Nodes { get; }

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
        public virtual bool IsPositionAvailableFor(IGraphNode targetNode, IActor actor)
        {
            if (targetNode == null)
            {
                throw new ArgumentNullException(nameof(targetNode));
            }

            if (actor == null)
            {
                throw new ArgumentNullException(nameof(actor));
            }

            var testTargetNodes = GetActorTestTargetNodes(actor, targetNode);

            foreach (var node in testTargetNodes)
            {
                var isAvailable = IsNodeAvailableForActor(node, actor);
                if (!isAvailable)
                {
                    return false;
                }
            }

            return true;
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
        public void ReleaseNode(IGraphNode node, IPassMapBlocker blocker)
        {
            if (!_nodeBlockers.TryGetValue(node, out IList<IPassMapBlocker> blockers))
            {
                throw new InvalidOperationException($"Попытка освободить узел {node}, который не заблокирован.");
            }

            if (!blockers.Contains(blocker))
            {
                throw new InvalidOperationException(
                    $"Попытка освободить узел {node}, который не заблокирован блокировщиком {blocker}.");
            }

            blockers.Remove(blocker);
        }

        /// <summary>
        /// Указывает, что узел карты занят блоком.
        /// </summary>
        /// <param name="node">Узел, который будет занят указанным блоком.</param>
        /// <param name="blocker">Блокер, который занимает узел.</param>
        public void HoldNode(IGraphNode node, IPassMapBlocker blocker)
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
        public abstract IEnumerable<IGraphNode> GetNext(IGraphNode node);

        /// <summary>
        /// Создаёт ребро между двумя узлами графа карты.
        /// </summary>
        /// <param name="node1">Узел графа карты.</param>
        /// <param name="node2">Узел графа карты.</param>
        public abstract void AddEdge(IGraphNode node1, IGraphNode node2);

        /// <summary>
        /// Удаляет ребро между двумя узлами графа карты.
        /// </summary>
        /// <param name="node1">Узел графа карты.</param>
        /// <param name="node2">Узел графа карты.</param>
        public abstract void RemoveEdge(IGraphNode node1, IGraphNode node2);

        /// <inheritdoc/>
        public abstract void AddNode(IGraphNode node);

        /// <inheritdoc/>
        public abstract void RemoveNode(IGraphNode node);

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
        public void FindPath(
            IGraphNode start,
            IGraphNode end,
            IAstarContext context,
            List<IGraphNode> outputPath)
        {
            if (start is null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (end is null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (outputPath is null)
            {
                throw new ArgumentNullException(nameof(outputPath));
            }

            var startNode = start;
            var finishNode = end;

            var astar = new AStar(context, startNode, finishNode);
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

        /// <inheritdoc/>
        public abstract bool IsPositionAvailableForContainer(IGraphNode targetNode);

        /// <inheritdoc/>
        public abstract int DistanceBetween(IGraphNode currentNode, IGraphNode targetNode);

        private IEnumerable<IGraphNode> GetActorTestTargetNodes(IActor actor, IGraphNode baseTargetNode)
        {
            yield return baseTargetNode;

            if (actor.Person.PhysicalSize == PhysicalSize.Size7)
            {
                var neighbors = GetNext(baseTargetNode);
                foreach (var neighbor in neighbors)
                {
                    yield return neighbor;
                }
            }
        }

        private bool IsNodeAvailableForActor(IGraphNode targetNode, IActor actor)
        {
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
    }
}