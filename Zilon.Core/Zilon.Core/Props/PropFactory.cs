using Zilon.Core.Schemes;

namespace Zilon.Core.Props
{
    public class PropFactory : IPropFactory
    {
        private readonly ISchemeService _schemeService;

        [ExcludeFromCodeCoverage]
        public PropFactory(ISchemeService schemeService)
        {
            _schemeService = schemeService;
        }

        public Equipment CreateEquipment(IPropScheme scheme)
        {
            if (scheme is null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            if (scheme.Equip == null)
            {
                throw new ArgumentException("Не корректная схема.", nameof(scheme));
            }

            if (scheme.Equip.ActSids == null)
            {
                return new Equipment(scheme, null);
            }

            var actSchemes = new List<ITacticalActScheme>();
            var actSchemeSids = scheme.Equip.ActSids;

            foreach (var actSchemeSid in actSchemeSids)
            {
                ITacticalActScheme actScheme = _schemeService.GetScheme<ITacticalActScheme>(actSchemeSid);

                actSchemes.Add(actScheme);
            }

            Equipment equipment = new Equipment(scheme, actSchemes);

            return equipment;
        }

        public Resource CreateResource(IPropScheme scheme, int count)
        {
            Resource resource = new Resource(scheme, count);

            return resource;
        }
    }
}