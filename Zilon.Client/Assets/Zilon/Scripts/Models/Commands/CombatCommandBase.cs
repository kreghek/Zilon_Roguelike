using Zilon.Logic.Tactics;

namespace Assets.Zilon.Scripts.Commands
{
    abstract class CombatCommandBase : ICommand
    {
        protected readonly Combat combat;

        public CombatCommandBase(Combat combat)
        {
            this.combat = combat;
        }

        public abstract void Execute();
    }
}
