using Assets.Zilon.Scripts.Services;
using Zenject;
using Zilon.Logic.Services;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICombatService>().To<CombatService>().AsSingle();
        Container.Bind<ICommandManager>().To<CombatCommandManager>().AsSingle();
    }
}