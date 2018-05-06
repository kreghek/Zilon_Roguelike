using Zilon.Logic.Tactics;

namespace Assets.Zilon.Scripts.Models.Commands
{
    interface ICombatCommandContext: ICommandContext
    {
        Combat Combat { get; set; }
    }
}
