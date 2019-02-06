﻿using System;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public class SectorFactory : ISectorFactory
    {
        private readonly IActorManager _actorManager;
        private readonly IPropContainerManager _propContainerManager;
        private readonly ITraderManager _traderManager;
        private readonly IDropResolver _dropResolver;
        private readonly ISchemeService _schemeService;

        public SectorFactory(IActorManager actorManager,
            IPropContainerManager propContainerManager,
            ITraderManager traderManager,
            IDropResolver dropResolver,
            ISchemeService schemeService)
        {
            _actorManager = actorManager ?? throw new ArgumentNullException(nameof(actorManager));
            _propContainerManager = propContainerManager ?? throw new ArgumentNullException(nameof(propContainerManager));
            _traderManager = traderManager;
            _dropResolver = dropResolver ?? throw new ArgumentNullException(nameof(dropResolver));
            _schemeService = schemeService ?? throw new ArgumentNullException(nameof(schemeService));
        }

        public ISector Create(IMap map)
        {
            var sector = new Sector(map,
                _actorManager,
                _propContainerManager,
                _traderManager,
                _dropResolver,
                _schemeService);
            return sector;
        }
    }
}
