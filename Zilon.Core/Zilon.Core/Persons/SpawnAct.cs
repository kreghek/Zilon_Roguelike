using JetBrains.Annotations;

using Zilon.Core.Common;
using Zilon.Core.Props;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class SpawnAct : TacticalAct, ISpawnAct
    {
        public SpawnAct(
            [NotNull] ITacticalActScheme scheme,
            [NotNull] Roll efficient,
            [NotNull] Roll toHit,
            [CanBeNull] Equipment equipment,
            [NotNull] IPersonScheme personScheme) : base(scheme, efficient, toHit, equipment)
        {
            PersonScheme = personScheme ?? throw new System.ArgumentNullException(nameof(personScheme));
        }

        public IPersonScheme PersonScheme { get; }
    }
}
