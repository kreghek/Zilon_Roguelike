using System;
using System.Collections.Generic;

using Zilon.Core.Graphs;
using Zilon.Core.StaticObjectModules;

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
        public bool IsMapBlock { get => GetIsMapBlock(); }

        /// <inheritdoc/>
        public void AddModule<TSectorObjectModule>(TSectorObjectModule sectorObjectModule)
        {
            var sanitizedModuleType = GetSanitizedModuleType(typeof(TSectorObjectModule));
            if (sanitizedModuleType is null)
            {
                throw new ArgumentException("Указанные объект не является модулем", nameof(sectorObjectModule));
            }

            _modules.Add(sanitizedModuleType, sectorObjectModule);
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
            var moduleAttribute = type.GetCustomAttributes(typeof(StaticObjectModuleAttribute), false);
            if (moduleAttribute is null)
            {
                var parent = type.BaseType;
                if (parent is null)
                {
                    return null;
                }
                else
                {
                    return GetSanitizedModuleType(parent);
                }
            }
            else
            {
                return type;
            }
        }
    }
}
