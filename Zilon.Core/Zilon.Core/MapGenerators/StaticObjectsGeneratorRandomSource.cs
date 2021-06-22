using System;
using System.Collections.Generic;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    public sealed class StaticObjectsGeneratorRandomSource : IStaticObjectsGeneratorRandomSource
    {
        private readonly IDice _dice;

        public StaticObjectsGeneratorRandomSource(IDice dice)
        {
            _dice = dice;
        }

        private static PropContainerPurpose GetPurposeByResourceType(SectorResourceType resourceType)
        {
            return resourceType switch
            {
                SectorResourceType.Aurihulk or
                SectorResourceType.Copper or
                SectorResourceType.Gold or
                SectorResourceType.Iron or
                SectorResourceType.Silver => PropContainerPurpose.OreDeposits,
                SectorResourceType.Stones => PropContainerPurpose.StoneDeposits,
                SectorResourceType.CherryBrushes => PropContainerPurpose.CherryBrush,
                SectorResourceType.WaterPuddles => PropContainerPurpose.Puddle,
                _ => throw new InvalidOperationException(),
            };
        }

        public PropContainerPurpose RollPurpose(IResourceDepositData resourceDepositData)
        {
            if (resourceDepositData is null)
            {
                throw new ArgumentNullException(nameof(resourceDepositData));
            }

            var purposeList = new List<PropContainerPurpose>(100);
            foreach (var resourceDepositDataItem in resourceDepositData.Items)
            {
                var count = (int)Math.Round(resourceDepositDataItem.Share * 100);
                var purpose = GetPurposeByResourceType(resourceDepositDataItem.ResourceType);
                for (var i = 0; i < count; i++)
                {
                    purposeList.Add(purpose);
                }
            }

            //if (purposeList.Count < 100)
            //{
            //    var diff = 100 - purposeList.Count;
            //    var everywherePurpose = new[]
            //        { PropContainerPurpose.Puddle, PropContainerPurpose.Pit, PropContainerPurpose.TrashHeap };
            //    var diffShare = (int)Math.Ceiling(diff / 2f);
            //    foreach (var purpose in everywherePurpose)
            //    {
            //        for (var i = 0; i < diffShare; i++)
            //        {
            //            purposeList.Add(purpose);
            //        }
            //    }
            //}
            //else
            //{
            //    purposeList.Add(PropContainerPurpose.TrashHeap);
            //    purposeList.Add(PropContainerPurpose.Puddle);
            //    purposeList.Add(PropContainerPurpose.Pit);
            //}

            return _dice.RollFromList(purposeList);
        }
    }
}