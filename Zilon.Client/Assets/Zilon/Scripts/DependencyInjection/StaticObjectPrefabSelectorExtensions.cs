using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Core.MapGenerators.StaticObjectFactories;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    public static class StaticObjectPrefabSelectorExtensions
    {
        public static void RegisterStaticObjecServices(this DiContainer diContainer)
        {
            diContainer.Bind<StaticObjectViewModelSelector>().AsSingle();

            diContainer.Bind<IStaticObjectFactoryCollector>().FromMethod(diFactory =>
            {
                var factories = diFactory.Container.ResolveAll<IStaticObjectFactory>().ToArray();
                return new StaticObjectFactoryCollector(factories);
            }).AsSingle();
            diContainer.Bind<IStaticObjectFactory>().To<StoneDepositFactory>().AsSingle();
            diContainer.Bind<IStaticObjectFactory>().To<OreDepositFactory>().AsSingle();
            diContainer.Bind<IStaticObjectFactory>().To<TrashHeapFactory>().AsSingle();
            diContainer.Bind<IStaticObjectFactory>().To<CherryBrushFactory>().AsSingle();
            diContainer.Bind<IStaticObjectFactory>().To<PitFactory>().AsSingle();
            diContainer.Bind<IStaticObjectFactory>().To<PuddleFactory>().AsSingle();
        }
    }
}
