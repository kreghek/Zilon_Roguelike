using Assets.Zilon.Scripts.Commands;

namespace Assets.Zilon.Scripts.Services
{
    class CombatCommandManager : ICommandManager<CombatCommandBase>
    {
        public CombatCommandBase Pop()
        {
            throw new System.NotImplementedException();
        }

        public void Push(CombatCommandBase command)
        {
            throw new System.NotImplementedException();
        }
    }
}
