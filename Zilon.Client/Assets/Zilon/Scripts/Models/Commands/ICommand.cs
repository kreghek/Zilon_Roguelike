namespace Assets.Zilon.Scripts.Models.Commands
{
    interface ICommand<in TContext> where TContext : class, ICommandContext
    {
        void Execute(TContext context);
    }
}
