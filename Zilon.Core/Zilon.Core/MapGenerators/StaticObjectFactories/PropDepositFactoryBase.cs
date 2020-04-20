using System;

using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public abstract class PropDepositFactoryBase : IStaticObjectFactory
    {
        private readonly string[] _toolTags;
        private readonly string _dropTableSchemeSid;
        private readonly ISchemeService _schemeService;
        private readonly IDropResolver _dropResolver;

        public PropDepositFactoryBase(
            string[] toolTags,
            string dropTableSchemeSid,
            PropContainerPurpose propContainerPurpose,
            ISchemeService schemeService,
            IDropResolver dropResolver)
        {
            _toolTags = toolTags ?? throw new ArgumentNullException(nameof(toolTags));
            _dropTableSchemeSid = dropTableSchemeSid ?? throw new ArgumentNullException(nameof(dropTableSchemeSid));
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            Purpose = propContainerPurpose;
        }

        public PropContainerPurpose Purpose { get; }

        public IStaticObject Create(ISector sector, HexNode node, int id)
        {
            if (sector is null)
            {
                throw new ArgumentNullException(nameof(sector));
            }

            var staticObject = new StaticObject(node, Purpose, id);

            // Все залежи изначально имеют пустой модуль контейнера.
            // Он будет заполняться по мере добычи.
            var containerModule = new DepositContainer();
            staticObject.AddModule(containerModule);

            var lifetimeModule = new LifetimeModule(sector.StaticObjectManager, staticObject);

            var dropScheme = _schemeService.GetScheme<IDropTableScheme>(_dropTableSchemeSid);
            var depositModule = new PropDepositModule(containerModule, dropScheme, _dropResolver, _toolTags, lifetimeModule);
            staticObject.AddModule(depositModule);

            return staticObject;
        }
    }
}
