using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Graphs;
using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Базовая реализация статического объекта в секторе.
    /// </summary>
    public sealed class StaticObject : IStaticObject
    {
        private readonly IDictionary<string, IStaticObjectModule> _modules;

        public StaticObject(IGraphNode node, int id)
        {
            _modules = new Dictionary<string, IStaticObjectModule>();

            Id = id;
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        /// <inheritdoc/>
        public int Id { get; }

        /// <inheritdoc/>
        public IGraphNode Node { get; }

        /// <inheritdoc/>
        public bool IsMapBlock { get => GetIsMapBlock(); }

        /// <inheritdoc/>
        public bool IsSightBlock { get => false; }

        /// <inheritdoc/>
        public void AddModule<TStaticObjectModule>(TStaticObjectModule sectorObjectModule) where TStaticObjectModule: IStaticObjectModule
        {
            _modules.Add(sectorObjectModule.Key, sectorObjectModule);
        }

        /// <inheritdoc/>
        public TStaticObjectModule GetModule<TStaticObjectModule>(string key) where TStaticObjectModule : IStaticObjectModule
        {
            return (TStaticObjectModule)_modules[key];
        }

        /// <inheritdoc/>
        public bool HasModule<TStaticObjectModule>(string key) where TStaticObjectModule : IStaticObjectModule
        {
            return _modules.ContainsKey(key);
        }

        /// <inheritdoc/>
        private bool GetIsMapBlock()
        {
            var propContainer = this.GetModuleSafe<IPropContainer>();
            return (propContainer?.IsMapBlock).GetValueOrDefault(true);
        }
    }
}
