using Zilon.Logic.Tactics;

namespace Assets.Zilon.Scripts.Models.Commands
{
    class CombatCommandContext : ICombatCommandContext
    {
        private Combat combat;

        public CombatCommandContext(Combat combat)
        {
            this.combat = combat;
        }

        public Combat Combat
        {
            get
            {
                return combat;
            }

            set
            {
                combat = value;
            }
        }
    }
}
