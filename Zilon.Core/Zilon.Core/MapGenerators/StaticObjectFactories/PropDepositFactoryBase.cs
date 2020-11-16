using System;

using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.StaticObjectFactories
{
    public abstract class PropDepositFactoryBase : IStaticObjectFactory
    {
        private readonly IDropResolver _dropResolver;
        private readonly string _dropTableSchemeSid;
        private readonly ISchemeService _schemeService;
        private readonly string[] _toolTags;

        protected PropDepositFactoryBase(
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

        protected abstract DepositMiningDifficulty Difficulty { get; }

        protected abstract int ExhausingValue { get; }

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

            var dropScheme = _schemeService.GetScheme<IDropTableScheme>(_dropTableSchemeSid);
            var depositModule = new PropDepositModule(containerModule, dropScheme, _dropResolver, _toolTags,
                ExhausingValue, Difficulty);
            staticObject.AddModule(depositModule);

            var lifetimeModule = new DepositLifetimeModule(sector.StaticObjectManager, staticObject);
            staticObject.AddModule(lifetimeModule);

            var durabilityModule = new DepositDurabilityModule(depositModule, lifetimeModule, 10);
            staticObject.AddModule(durabilityModule);

            return staticObject;
        }
    }
}