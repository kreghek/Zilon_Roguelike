using Zenject;

using Zilon.Core.PersonGeneration;
using Zilon.Core.Persons;
using Zilon.Core.Scoring;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    public static class HumanPersonFactoryExtensions
    {
        public static void RegisterPersonFactory(this DiContainer diContainer)
        {
            diContainer.Bind<IPersonFactory>().To<RandomHumanPersonFactory>().AsSingle()
                .OnInstantiated<RandomHumanPersonFactory>((injectContext, service) => {
                    service.PlayerEventLogService = injectContext.Container.Resolve<IPlayerEventLogService>();
                });
            diContainer.Bind<IMonsterPersonFactory>().To<MonsterPersonFactory>().AsSingle();
            diContainer.Bind<IPersonPerkInitializator>().To<PersonPerkInitializator>().AsSingle();
        }
    }
}
