using System;
using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Базовая реализация статического объекта в секторе.
    /// </summary>
    public sealed class StaticObject : IStaticObject
    {
        private readonly IDictionary<string, IStaticObjectModule> _modules;

        public StaticObject(IGraphNode node, PropContainerPurpose purpose, int id)
        {
            _modules = new Dictionary<string, IStaticObjectModule>();

            Id = id;
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Purpose = purpose;
        }

        /// <inheritdoc/>
        public int Id { get; }

        /// <inheritdoc/>
        public IGraphNode Node { get; }

        /// <inheritdoc/>
        public bool IsMapBlock => GetIsMapBlock();

        /// <inheritdoc/>
        public bool IsSightBlock => false;

        /// <inheritdoc/>
        public PropContainerPurpose Purpose { get; }

        public PhysicalSize PhysicalSize => PhysicalSize.Size1;

        /// <inheritdoc/>
        public void AddModule<TStaticObjectModule>(TStaticObjectModule sectorObjectModule)
            where TStaticObjectModule : IStaticObjectModule
        {
            _modules.Add(sectorObjectModule.Key, sectorObjectModule);
        }

        public bool CanBeDamaged()
        {
            var durabilityModule = this.GetModuleSafe<IDurabilityModule>();
            return durabilityModule != null && durabilityModule.Value > 0;
        }

        /// <inheritdoc/>
        public TStaticObjectModule GetModule<TStaticObjectModule>(string key)
            where TStaticObjectModule : IStaticObjectModule
        {
            return (TStaticObjectModule)_modules[key];
        }

        /// <inheritdoc/>
        public bool HasModule(string key)
        {
            return _modules.ContainsKey(key);
        }

        public void TakeDamage(int value)
        {
            var durabilityModule = this.GetModuleSafe<IDurabilityModule>();
            if (durabilityModule is null)
            {
                throw new InvalidOperationException("Attempt to damage object with no durability module.");
            }

            durabilityModule.TakeDamage(value);
        }

        /// <inheritdoc/>
        private bool GetIsMapBlock()
        {
            var propContainer = this.GetModuleSafe<IPropContainer>();
            return (propContainer?.IsMapBlock).GetValueOrDefault(true);
        }
    }
}