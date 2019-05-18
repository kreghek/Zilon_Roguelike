using Zenject;

using Zilon.Bot.Players;

namespace Assets.Zilon.Scripts.Services
{
    class ZenjectLogicStateFactory : ILogicStateFactory
    {
        private readonly DiContainer _diContainer;

        public ZenjectLogicStateFactory(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public ILogicState CreateLogic<T>() where T : ILogicState
        {
            return _diContainer.Resolve<T>();
        }

        public ILogicStateTrigger CreateTrigger<T>() where T : ILogicStateTrigger
        {
            return _diContainer.Resolve<T>();
        }
    }
}
