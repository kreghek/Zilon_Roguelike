using Zenject;

using Zilon.Core.Persons;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    public static class HumanPersonFactoryExtensions
    {
        public static void RegisterHumanPersonFactory(this DiContainer diContainer)
        {
            diContainer.Bind<IHumanPersonFactory>().To<RandomHumanPersonFactory>().AsSingle();
            diContainer.Bind<IPersonPerkInitializator>().To<PersonPerkInitializator>().AsSingle();
        }
    }
}
