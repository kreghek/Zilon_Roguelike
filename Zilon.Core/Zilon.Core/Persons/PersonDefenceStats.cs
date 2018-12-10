using System;

using JetBrains.Annotations;

namespace Zilon.Core.Persons
{
    public class PersonDefenceStats : IPersonDefenceStats
    {
        public PersonDefenceStats([NotNull] [ItemNotNull] PersonDefenceItem[] defences,
            [NotNull] [ItemNotNull] PersonArmorItem[] armors)
        {
            Defences = defences ?? throw new ArgumentNullException(nameof(defences));
            Armors = armors ?? throw new ArgumentNullException(nameof(armors));
        }

        public PersonDefenceItem[] Defences { get; }

        public PersonArmorItem[] Armors { get; private set; }

        public void SetArmors([ItemNotNull, NotNull] PersonArmorItem[] armors)
        {
            Armors = armors ?? throw new ArgumentNullException(nameof(armors));
        }
    }
}
