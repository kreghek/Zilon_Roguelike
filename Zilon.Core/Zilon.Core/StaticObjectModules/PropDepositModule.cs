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

        public PropDepositModule(IPropContainer propContainer, IDropTableScheme dropTableScheme, IDropResolver dropResolver)
        {
            _propContainer = propContainer ?? throw new ArgumentNullException(nameof(propContainer));
            _dropTableScheme = dropTableScheme ?? throw new ArgumentNullException(nameof(dropTableScheme));
            _dropResolver = dropResolver;
        }

        public IPropScheme Tool { get; }
        public bool IsExhausted { get; }

        public void Mine()
        {
            var props = _dropResolver.Resolve(new[] { _dropTableScheme });
            foreach (var prop in props)
            {
                _propContainer.Content.Add(prop);
            }
        }
    }
}
