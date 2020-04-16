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
        public void AddModule<TSectorObjectModule>(TSectorObjectModule sectorObjectModule) where TSectorObjectModule: IStaticObjectModule
        {
            _modules.Add(sectorObjectModule.Key, sectorObjectModule);
        }

        /// <inheritdoc/>
        public TSectorObjectModule GetModule<TSectorObjectModule>()
        {
            return (TSectorObjectModule)_modules[typeof(TSectorObjectModule)];
        }

        /// <inheritdoc/>
        public bool HasModule<TSectorObjectModule>()
        {
            return _modules.ContainsKey(typeof(TSectorObjectModule));
        }

        /// <inheritdoc/>
        private bool GetIsMapBlock()
        {
            var propContainer = this.GetModuleSafe<IPropContainer>();
            return (propContainer?.IsMapBlock).GetValueOrDefault(true);
        }

        private Type GetSanitizedModuleType(Type type)
        {
            var implementedInterfaces = type.GetInterfaces();

            foreach (var implementedInterface in implementedInterfaces)
            {
                //var moduleAttribute = implementedInterface.GetCustomAttributes(typeof(StaticObjectModuleAttribute), false);
                var moduleAttribute = implementedInterface.CustomAttributes.SingleOrDefault(x => x.AttributeType == typeof(StaticObjectModuleAttribute));
                if (moduleAttribute is null)
                {
                    continue;
                }
                else
                {
                    return implementedInterface;
                }
            }

            return null;
        }
    }
}
