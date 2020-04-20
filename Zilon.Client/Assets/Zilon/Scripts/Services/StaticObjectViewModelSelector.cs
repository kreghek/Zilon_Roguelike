using UnityEngine;

using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    class StaticObjectViewModelSelector
    {
        public StaticObjectViewModel SelectViewModel(IStaticObject staticObject)
        {
            if (staticObject.HasModule<IPropDepositModule>())
            {
                // Возвращаем представление обычного статика-камня.
                return LoadFromResource("StoneDeposit");
            }
            else
            {
                if (staticObject.GetModule<IPropContainer>() is ILootContainer)
                {
                    return LoadFromResource("Loot");
                }

                if (staticObject.GetModule<IPropContainer>().Purpose == PropContainerPurpose.Treasures)
                {
                    return LoadFromResource("Chest");
                }

                return LoadFromResource("Trash");
            }
        }

        private StaticObjectViewModel LoadFromResource(string prefabName)
        {
            return Resources.Load<StaticObjectViewModel>($"StaticObjects/{prefabName}");
        }
    }
}
