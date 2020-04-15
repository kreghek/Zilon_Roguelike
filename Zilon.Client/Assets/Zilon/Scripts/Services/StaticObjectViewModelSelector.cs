using System;

using JetBrains.Annotations;

using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    class StaticObjectViewModelSelector
    {
        [NotNull] public ContainerVm ChestPrefab;

        [NotNull] public ContainerVm LootPrefab;

        [NotNull] public ContainerVm TrashPrefab;

        public StaticObjectViewModel SelectViewModel(IStaticObject staticObject)
        {
            if (staticObject.HasModule<IPropContainer>())
            {

                if (staticObject.GetModule<IPropContainer>() is ILootContainer)
                {
                    return LootPrefab;
                }

                if (staticObject.GetModule<IPropContainer>().Purpose == PropContainerPurpose.Treasures)
                {
                    return ChestPrefab;
                }

                return TrashPrefab;
            }
            else
            {
                // Возвращаем представление обычного статика-камня.
                throw new NotImplementedException();
            }
        }
    }
}
