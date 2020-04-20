using System;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Core.StaticObjectModules
{
    public class PropDepositModule : IPropDepositModule
    {
        private readonly IPropContainer _propContainer;
        private readonly IDropTableScheme _dropTableScheme;
        private readonly IDropResolver _dropResolver;
        private readonly ILifetimeModule _lifetimeModule;
        private readonly string[] _toolTags;

        private int _exhautingCounter = 10;

        public PropDepositModule(IPropContainer propContainer,
            IDropTableScheme dropTableScheme,
            IDropResolver dropResolver,
            string[] toolTags,
            ILifetimeModule lifetimeModule)
        {
            _propContainer = propContainer ?? throw new ArgumentNullException(nameof(propContainer));
            _dropTableScheme = dropTableScheme ?? throw new ArgumentNullException(nameof(dropTableScheme));
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _toolTags = toolTags ?? throw new ArgumentNullException(nameof(toolTags));
            _lifetimeModule = lifetimeModule ?? throw new ArgumentNullException(nameof(lifetimeModule));
        }

        /// <inheritdoc/>
        public string[] GetToolTags()
        {
            return _toolTags;
        }

        /// <inheritdoc/>
        public bool IsExhausted { get => _exhautingCounter > 0; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public string Key { get => nameof(IPropDepositModule); }

        /// <inheritdoc/>
        public void Mine()
        {
            if (_exhautingCounter <= 0)
            {
                throw new InvalidOperationException("Попытка выполнить добычу в исчерпанных залежах");
            }

            var props = _dropResolver.Resolve(new[] { _dropTableScheme });
            foreach (var prop in props)
            {
                _propContainer.Content.Add(prop);
            }

            _exhautingCounter--;

            if (_exhautingCounter <= 0)
            {
                _lifetimeModule.Destroy();
            }
            else
            {
                _propContainer.IsActive = true;
            }
        }
    }
}
