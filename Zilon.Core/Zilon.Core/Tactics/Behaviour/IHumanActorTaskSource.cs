using System.Collections.Generic;

using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics.Behaviour
{
    public interface IHumanActorTaskSource: IActorTaskSource
    {
        /// <summary>
        /// Переключает текущего ключевого актёра.
        /// </summary>
        /// <param name="currentActor"> Целевой клчевой актёр. </param>
        void SwitchActor(IActor currentActor);

        /// <summary>
        /// Текущий активный ключевой актёр.
        /// </summary>
        IActor CurrentActor { get; }

        void IntentAttack(IAttackTarget target);
        void IntentEquip(Equipment equipment, int slotIndex);
        void IntentMove(HexNode targetNode);
        void IntentOpenContainer(IPropContainer container, IOpenContainerMethod method);
        void IntentTransferProps(IEnumerable<PropTransfer> transfers);
        void IntentUseSelf(IProp usableProp);
    }
}