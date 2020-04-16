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

        private int _exhautingCounter = 10;

        public PropDepositModule(IPropContainer propContainer,
            IDropTableScheme dropTableScheme,
            IDropResolver dropResolver,
            IPropScheme toolScheme)
        {
            _propContainer = propContainer ?? throw new ArgumentNullException(nameof(propContainer));
            _dropTableScheme = dropTableScheme ?? throw new ArgumentNullException(nameof(dropTableScheme));
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));

            Tool = toolScheme ?? throw new ArgumentNullException(nameof(toolScheme));
        }

        public IPropScheme Tool { get; }

        public bool IsExhausted { get => _exhautingCounter > 0; }

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

            _exhautingCounter++;
        }
    }
}
