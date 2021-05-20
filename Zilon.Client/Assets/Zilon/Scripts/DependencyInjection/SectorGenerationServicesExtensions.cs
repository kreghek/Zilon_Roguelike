﻿using Assets.Zilon.Scripts.Services;

using Zenject;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Assets.Zilon.Scripts.DependencyInjection
{
    public static class SectorGenerationServicesExtensions
    {
        public static void RegisterGenerationServices(this DiContainer diContainer)
        {
            diContainer.Bind<ISectorGenerator>().To<SectorGenerator>().AsSingle();
            diContainer.Bind<IMapFactory>().WithId("room").To<RoomMapFactory>().AsSingle();
            diContainer.Bind<IMapFactory>().WithId("cave").To<CellularAutomatonMapFactory>().AsSingle();
            diContainer.Bind<IMapFactorySelector>().To<MapFactorySelector>().AsSingle();
            diContainer.Bind<IRoomGeneratorRandomSource>().FromMethod(context =>
            {
                var linearDice = context.Container.ResolveId<IDice>("linear");
                var roomSizeDice = context.Container.ResolveId<IDice>("exp");
                return new RoomGeneratorRandomSource(linearDice, roomSizeDice);
            }).AsSingle();
            diContainer.Bind<IInteriorObjectRandomSource>().To<InteriorObjectRandomSource>().AsSingle();
            diContainer.Bind<IRoomGenerator>().To<RoomGenerator>().AsSingle();
            diContainer.Bind<IChestGenerator>().To<ChestGenerator>().AsSingle();
            diContainer.Bind<IChestGeneratorRandomSource>().To<ChestGeneratorRandomSource>().AsSingle();
            diContainer.Bind<IMonsterGenerator>().To<MonsterGenerator>().AsSingle();
            diContainer.Bind<IMonsterGeneratorRandomSource>().To<MonsterGeneratorRandomSource>().AsSingle();
            diContainer.Bind<NationalUnityEventService>().AsSingle();
            diContainer.Bind<ISectorFactory>().To<SectorFactory>().AsSingle().OnInstantiated<SectorFactory>((context, service) =>
            {
                service.NationalUnityEventService = context.Container.Resolve<NationalUnityEventService>();
                service.ScoreManager = context.Container.Resolve<IScoreManager>();
            });

            diContainer.Bind<IStaticObstaclesGenerator>().To<StaticObstaclesGenerator>().AsSingle();
            diContainer.Bind<IStaticObjectsGeneratorRandomSource>().To<StaticObjectsGeneratorRandomSource>().AsSingle();
            diContainer.Bind<IResourceMaterializationMap>().To<ResourceMaterializationMap>().AsSingle();
        }
    }
}
