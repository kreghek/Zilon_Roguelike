using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public abstract class EquipmentCarrierBase : IEquipmentCarrier
    {
        private readonly Equipment[] _equipment;

        protected EquipmentCarrierBase(IEnumerable<Equipment> equipments)
        {
            _equipment = equipments.ToArray();
        }

        protected EquipmentCarrierBase(int size)
        {
            _equipment = new Equipment[size];
        }

        public virtual Equipment this[int index]
        {
            get => _equipment[index];
            set => _equipment[index] = value;
        }

        public abstract PersonSlotSubScheme[] Slots { get; }

        public abstract event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;

        public IEnumerator<Equipment> GetEnumerator() => _equipment.Cast<Equipment>().GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => _equipment.GetEnumerator();
    }
}
