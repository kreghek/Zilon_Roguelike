using System;
using System.Linq;

namespace Zilon.Core.Tactics
{
    public static class ActUsageHelper
    {
        public static bool SectorHasAttackTarget(ISector sector, IAttackTarget target)
        {
            switch (target)
            {
                case IActor actor:
                    return SectorHasAttackedActor(sector, actor);

                case IStaticObject staticObject:
                    return SectorHasAttackedStaticObject(sector, staticObject);

                default:
                    throw new InvalidOperationException($"Unknown attack target type {target.GetType().FullName}.");
            }
        }

        private static bool SectorHasAttackedActor(ISector sector, IAttackTarget target)
        {
            if (sector.ActorManager is null)
            {
                // In test environment not all sector mocks has actor manager
                return true;
            }

            return sector.ActorManager.Items.Any(x => ReferenceEquals(x, target));
        }

        private static bool SectorHasAttackedStaticObject(ISector sector, IAttackTarget target)
        {
            if (sector.StaticObjectManager is null)
            {
                // In test environment not all sector mocks has actor manager
                return true;
            }

            return sector.StaticObjectManager.Items.Any(x => ReferenceEquals(x, target));
        }
    }
}