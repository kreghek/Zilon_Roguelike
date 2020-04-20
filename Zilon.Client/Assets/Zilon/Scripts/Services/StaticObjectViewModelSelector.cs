using System;

using UnityEngine;

using Zilon.Core.Tactics;

namespace Assets.Zilon.Scripts.Services
{
    class StaticObjectViewModelSelector
    {
        public StaticObjectViewModel SelectViewModel(IStaticObject staticObject)
        {
            switch (staticObject.Purpose)
            {
                // Модели залежей ресурсов
                case PropContainerPurpose.OreDeposits:
                    return LoadFromResource("OreDeposit");
                case PropContainerPurpose.StoneDeposits:
                    return LoadFromResource("StoneDeposit");
                case PropContainerPurpose.TrashHeap:
                    return LoadFromResource("TrashHeap");
                case PropContainerPurpose.Puddle:
                    return LoadFromResource("WaterPuddle");
                case PropContainerPurpose.CherryBrush:
                    return LoadFromResource("CherryBrush");

                // Модели статических препятсвий
                case PropContainerPurpose.Pit:
                    return LoadFromResource("Pit");

                // Модели контейнеров-сундуков
                case PropContainerPurpose.Trash:
                    return LoadFromResource("Trash");
                case PropContainerPurpose.Treasures:
                    return LoadFromResource("Treasures");
                case PropContainerPurpose.Loot:
                    return LoadFromResource("Loot");

                case PropContainerPurpose.Undefined:
                default:
                    throw new InvalidOperationException($"Статический объект имеет неизвестное назначение {staticObject.Purpose}");
            }
        }

        private StaticObjectViewModel LoadFromResource(string prefabName)
        {
            return Resources.Load<StaticObjectViewModel>($"StaticObjects/{prefabName}");
        }
    }
}
