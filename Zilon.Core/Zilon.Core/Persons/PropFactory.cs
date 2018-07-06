using System;

namespace Zilon.Core.Persons
{
    using Zilon.Core.Schemes;

    public class PropFactory
    {
        public Equipment CreateEquipment(PropScheme scheme)
        {
            if (scheme.Equip == null)
            {
                throw new ArgumentException("Не корректная схема.", nameof(scheme));
            }


            var actSchemeSids = scheme.Equip.ActSids;

            if (scheme.Equip.ActSids != null)
            {
                foreach (var actSchemeSid in actSchemeSids)
                {

                    var actScheme = _scheme

                    var act = new TacticalAct(actScheme);
                    actList.Add(act);
                }


                var equipment = new Equipment(scheme);

                return equipment;
            }
            else
            {
                return new Equipment(scheme, null);
            }
        }
    }
}
