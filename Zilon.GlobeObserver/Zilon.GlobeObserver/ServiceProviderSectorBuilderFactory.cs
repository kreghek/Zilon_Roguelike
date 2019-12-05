﻿using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Core.MapGenerators;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.GlobeObserver
{
    internal sealed class ServiceProviderSectorBuilderFactory : ISectorBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapFactory _mapFactory;
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;
        private readonly IEquipmentDurableService _equipmentDurableService;

        public ServiceProviderSectorBuilderFactory(
            IServiceProvider serviceProvider,
            IMapFactory mapFactory,
            IDropResolver dropResolver,
            ISchemeService schemeService,
            IEquipmentDurableService equipmentDurableService)
        {
            _serviceProvider = serviceProvider;
            _mapFactory = mapFactory;
            _dropResolver = dropResolver;
            _schemeService = schemeService;
            _equipmentDurableService = equipmentDurableService;
        }

        public ISectorBuilder GetBuilder()
        {
            var actorManager = _serviceProvider.GetRequiredService<IActorManager>();
            var propContainerManager = _serviceProvider.GetRequiredService<IPropContainerManager>();

            return new SectorBuilder(
                _mapFactory,
                actorManager,
                propContainerManager,
                _dropResolver,
                _schemeService,
                _equipmentDurableService);
        }
    }
}
