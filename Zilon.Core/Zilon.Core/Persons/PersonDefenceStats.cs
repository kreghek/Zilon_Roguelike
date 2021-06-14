using System;

namespace Zilon.Core.Persons
{
    public class PersonDefenceStats : IPersonDefenceStats
    {
        public PersonDefenceStats(PersonDefenceItem[] defences,
            PersonArmorItem[] armors)
        {
            Defences = defences ?? throw new ArgumentNullException(nameof(defences));
            Armors = armors ?? throw new ArgumentNullException(nameof(armors));
        }

        public PersonDefenceItem[] Defences { get; }

        public PersonArmorItem[] Armors { get; private set; }

        public void SetArmors(PersonArmorItem[] armors)
        {
            Armors = armors ?? throw new ArgumentNullException(nameof(armors));
        }
    }
}