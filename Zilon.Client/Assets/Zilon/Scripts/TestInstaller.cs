using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Services;
using Assets.Zilon.Scripts.Services.CombatScene;
using Zenject;
using Zilon.Logic.Services;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICombatService>().To<CombatService>().AsSingle();
        Container.Bind<ICommandManager>().To<CombatCommandManager>().AsSingle();
        Container.Bind<ICombatManager>().To<CombatManager>().AsSingle();
        Container.Bind<IPersonCommandHandler>().To<PersonCommandHandler>().AsSingle();
    }
}