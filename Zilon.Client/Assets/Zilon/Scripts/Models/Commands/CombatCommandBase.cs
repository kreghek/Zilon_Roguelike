namespace Assets.Zilon.Scripts.Models.Commands
{
    abstract class CombatCommandBase : ICommand<ICombatCommandContext>
    {
        public abstract void Execute(ICombatCommandContext context);
    }
}
