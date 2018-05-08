using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Services;
using Assets.Zilon.Scripts.Services.CombatScene;
using Zenject;
using Zilon.Logic.Services;
using Zilon.Logic.Services.CombatEvents;

public class TestInstaller : MonoInstaller<TestInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<ICombatService>().To<CombatService>().AsSingle();
        Container.Bind<ICommandManager>().To<CombatCommandManager>().AsSingle();
        Container.Bind<ICombatManager>().To<CombatManager>().AsSingle();
        Container.Bind<IPersonCommandHandler>().To<PersonCommandHandler>().AsSingle();
        Container.Bind<IEventManager>().To<EventManager>().AsSingle();
        Container.Bind<ICombatCommandResolver>().To<CombatCommandResolver>().AsSingle();
    }
}