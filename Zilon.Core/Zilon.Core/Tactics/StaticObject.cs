using System;
using System.Collections.Generic;

using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Базовая реализация статического объекта в секторе.
    /// </summary>
    public sealed class StaticObject: IStaticObject
    {
        private readonly IDictionary<Type, object> _modules;

        public StaticObject(IGraphNode node, int id)
        {
            _modules = new Dictionary<Type, object>();

            Id = id;
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        /// <inheritdoc/>
        public int Id { get; }

        /// <inheritdoc/>
        public IGraphNode Node { get; }

        /// <inheritdoc/>
        public void AddModule<TSectorObjectModule>(TSectorObjectModule sectorObjectModule)
        {
            _modules.Add(typeof(TSectorObjectModule), sectorObjectModule);
        }

        /// <inheritdoc/>
        public TSectorObjectModule GetModule<TSectorObjectModule>()
        {
            return (TSectorObjectModule)_modules[typeof(TSectorObjectModule)];
        }

        public bool HasModule<TSectorObjectModule>()
        {
            return _modules.ContainsKey(typeof(TSectorObjectModule));
        }
    }
}
