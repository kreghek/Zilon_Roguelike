using Zilon.Core.Tactics.Map;

namespace Zilon.Core.Tactics.Behaviour
{
    public class HumanBehaviourSource: IBehaviourSource
    {
        private IActor _currentActor;
        private ICommand _currentCommand;

        public HumanBehaviourSource(IActor startActor)
        {
            _currentActor = startActor;
        }
        
        public ICommand[] GetCommands(CombatMap map, IActor[] actors)
        {
            return null;
        }
    }
}