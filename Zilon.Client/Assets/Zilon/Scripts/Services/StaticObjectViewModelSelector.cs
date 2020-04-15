using UnityEngine;

using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    class StaticObjectViewModelSelector
    {
        public StaticObjectViewModel SelectViewModel(IStaticObject staticObject)
        {
            if (staticObject.HasModule<IPropContainer>())
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
            else
            {
                // Возвращаем представление обычного статика-камня.
                return LoadFromResource("StoneDeposit");
            }
        }

        private StaticObjectViewModel LoadFromResource(string prefabName)
        {
            return Resources.Load<StaticObjectViewModel>($"StaticObjects/{prefabName}");
        }
    }
}
