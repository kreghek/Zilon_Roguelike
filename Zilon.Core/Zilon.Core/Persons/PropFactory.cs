using System;
using System.Collections.Generic;

using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class PropFactory : IPropFactory
    {
        private readonly ISchemeService _schemeService;

        public PropFactory(ISchemeService schemeService)
        {
            _schemeService = schemeService;
        }

        public Equipment CreateEquipment(PropScheme scheme)
        {
            if (scheme.Equip == null)
            {
                throw new ArgumentException("Не корректная схема.", nameof(scheme));
            }


            var actList = new List<ITacticalAct>();
            var actSchemeSids = scheme.Equip.ActSids;

            if (scheme.Equip.ActSids != null)
            {
                foreach (var actSchemeSid in actSchemeSids)
                {

                    var actScheme = _schemeService.GetScheme<TacticalActScheme>(actSchemeSid);

                    var act = new TacticalAct(scheme.Equip.Power, actScheme);
                    actList.Add(act);
                }


                var equipment = new Equipment(scheme, actList.ToArray());

                return equipment;
            }
            else
            {
                return new Equipment(scheme, null);
            }
        }
    }
}
