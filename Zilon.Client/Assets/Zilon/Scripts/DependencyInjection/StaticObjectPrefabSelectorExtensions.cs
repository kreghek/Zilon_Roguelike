using Assets.Zilon.Scripts.Services;

using Zenject;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    public static class StaticObjectPrefabSelectorExtensions
    {
        public static void RegisterStaticObjecServices(this DiContainer diContainer)
        {
            diContainer.Bind<StaticObjectViewModelSelector>().AsSingle();
        }
    }
}
