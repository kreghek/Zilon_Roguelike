using System;

using LightInject;

using Zilon.Bot.Players;
using Zilon.Core.Client;
using Zilon.Core.CommonServices.Dices;
using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.RoomStyle;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.IoC;

namespace Zilon.Emulation.Common
{
    public abstract class InitialzationBase
    {
        public int? DiceSeed { get; set; }

        protected InitialzationBase()
        { 
        }

        protected InitialzationBase(int diceSeed)
        {
            DiceSeed = diceSeed;
        }

        public virtual void RegisterServices(IServiceRegistry serviceRegistry)
        {
            RegisterSchemeService(serviceRegistry);
            RegisterAuxServices(serviceRegistry);
            RegisterPlayerServices(serviceRegistry);

            RegisterSectorServices(serviceRegistry);
        }

        public abstract void ConfigureAux(IServiceFactory serviceFactory);

        private void RegisterSectorServices(IServiceRegistry serviceRegistry)
        {
            RegisterClientServices(serviceRegistry);
            RegisterGameLoop(serviceRegistry);
            RegisterScopedSectorService(serviceRegistry);
            RegisterBot(serviceRegistry);
        }

        private void RegisterSchemeService(IServiceRegistry container)
        {
            container.Register<ISchemeLocator>(factory =>
            {
                //TODO Организовать отдельный общий метод/класс/фабрику для конструирования локатора схем.
                // Подобные конструкции распределены по всему проекту: в тестах, бенчах, окружении ботов.
                // Следует их объединить в одном месте.
                var schemePath = Environment.GetEnvironmentVariable("ZILON_LIV_SCHEME_CATALOG");

                var schemeLocator = new FileSchemeLocator(schemePath);

                return schemeLocator;
            }, LightInjectWrapper.CreateSingleton());

            container.Register<ISchemeService, SchemeService>(LightInjectWrapper.CreateSingleton());

            container.Register<ISchemeServiceHandlerFactory, SchemeServiceHandlerFactory>(LightInjectWrapper.CreateSingleton());
        }

        private void RegisterScopedSectorService(IServiceRegistry container)
        {
            //TODO сделать генераторы независимыми от сектора.
            // Такое время жизни, потому что в зависимостях есть менеджеры.
            container.Register<ISectorGenerator, SectorGenerator>(LightInjectWrapper.CreateScoped());
            container.Register<IMonsterGenerator, MonsterGenerator>(LightInjectWrapper.CreateScoped());
            container.Register<IChestGenerator, ChestGenerator>(LightInjectWrapper.CreateScoped());
            container.Register<ICitizenGenerator, CitizenGenerator>(LightInjectWrapper.CreateScoped());
            container.Register<ISectorFactory, SectorFactory>(LightInjectWrapper.CreateScoped());
            container.Register<ISectorManager, InfiniteSectorManager>(LightInjectWrapper.CreateScoped());
            container.Register<IActorManager, ActorManager>(LightInjectWrapper.CreateScoped());
            container.Register<IPropContainerManager, PropContainerManager>(LightInjectWrapper.CreateScoped());
            container.Register<ITacticalActUsageService, TacticalActUsageService>(LightInjectWrapper.CreateScoped());
            container.Register<IActorTaskSource, MonsterBotActorTaskSource>("monster", LightInjectWrapper.CreateScoped());
        }

        private void RegisterGameLoop(IServiceRegistry container)
        {
            container.Register<IGameLoop, GameLoop>(LightInjectWrapper.CreateScoped());
        }

        /// <summary>
        /// Подготовка дополнительных сервисов
        /// </summary>
        private void RegisterAuxServices(IServiceRegistry container)
        {
            var linearDice = CreateRandomSeedAndLinearDice();
            var gaussDice = CreateRandomSeedAndGaussDice();
            var expDice = CreateRandomSeedAndExpDice();
            container.Register(factory => linearDice, "linear", LightInjectWrapper.CreateSingleton());
            container.Register(factory => gaussDice, "gauss", LightInjectWrapper.CreateSingleton());
            container.Register(factory => expDice, "exp", LightInjectWrapper.CreateSingleton());
            container.Register(factory=> factory.GetInstance<IDice>("linear"), LightInjectWrapper.CreateSingleton());
            container.Register<IDecisionSource, DecisionSource>(LightInjectWrapper.CreateSingleton());
            container.Register<ITacticalActUsageRandomSource, TacticalActUsageRandomSource>(LightInjectWrapper.CreateSingleton());
            container.Register<IPerkResolver, PerkResolver>(LightInjectWrapper.CreateSingleton());
            container.Register<IPropFactory, PropFactory>(LightInjectWrapper.CreateSingleton());
            container.Register<IDropResolver, DropResolver>(LightInjectWrapper.CreateSingleton());
            container.Register<IDropResolverRandomSource, DropResolverRandomSource>(LightInjectWrapper.CreateSingleton());
            container.Register<ISurvivalRandomSource, SurvivalRandomSource>(LightInjectWrapper.CreateSingleton());
            container.Register<IEquipmentDurableService, EquipmentDurableService>(LightInjectWrapper.CreateSingleton());
            container.Register<IEquipmentDurableServiceRandomSource, EquipmentDurableServiceRandomSource>(LightInjectWrapper.CreateSingleton());
            container.Register<IHumanPersonFactory, RandomHumanPersonFactory>(LightInjectWrapper.CreateSingleton());

            container.Register<IMapFactorySelector, LightInjectSwitchMapFactorySelector>(LightInjectWrapper.CreateSingleton());
            container.Register<IMapFactory, RoomMapFactory>("room", LightInjectWrapper.CreateSingleton());
            container.Register<IRoomGenerator, RoomGenerator>(LightInjectWrapper.CreateSingleton());
            container.Register(CreateRoomGeneratorRandomSource, LightInjectWrapper.CreateSingleton());
            container.Register<IMapFactory, CellularAutomatonMapFactory>("cellular-automaton", LightInjectWrapper.CreateSingleton());
            container.Register<IInteriorObjectRandomSource, InteriorObjectRandomSource>();
            container.Register<IMonsterGeneratorRandomSource, MonsterGeneratorRandomSource>(LightInjectWrapper.CreateSingleton());
            container.Register<IChestGeneratorRandomSource, ChestGeneratorRandomSource>(LightInjectWrapper.CreateSingleton());
            container.Register<ICitizenGeneratorRandomSource, CitizenGeneratorRandomSource>(LightInjectWrapper.CreateSingleton());
        }

        private static IRoomGeneratorRandomSource CreateRoomGeneratorRandomSource(IServiceFactory factory)
        {
            var localLinearDice = factory.GetInstance<IDice>("linear");
            var localRoomSizeDice = factory.GetInstance<IDice>("exp");
            var randomSource = new RoomGeneratorRandomSource(localLinearDice, localRoomSizeDice);
            return randomSource;
        }

        /// <summary>
        /// Создаёт кость и фиксирует зерно рандома.
        /// Если Зерно рандома не задано, то оно выбирается случайно.
        /// </summary>
        /// <returns> Экземпляр кости на основе выбранного или указанного ерна рандома. </returns>
        private IDice CreateRandomSeedAndLinearDice()
        {
            IDice dice;
            if (DiceSeed == null)
            {
                var diceSeedFact = new Random().Next(int.MaxValue);
                DiceSeed = diceSeedFact;
                dice = new LinearDice(diceSeedFact);
            }
            else
            {
                dice = new LinearDice(DiceSeed.Value);
            }

            return dice;
        }

        /// <summary>
        /// Создаёт кость и фиксирует зерно рандома.
        /// Если Зерно рандома не задано, то оно выбирается случайно.
        /// </summary>
        /// <returns> Экземпляр кости на основе выбранного или указанного ерна рандома. </returns>
        private IDice CreateRandomSeedAndGaussDice()
        {
            IDice dice;
            if (DiceSeed == null)
            {
                var diceSeedFact = new Random().Next(int.MaxValue);
                DiceSeed = diceSeedFact;
                dice = new GaussDice(diceSeedFact);
            }
            else
            {
                dice = new GaussDice(DiceSeed.Value);
            }

            return dice;
        }

        /// <summary>
        /// Создаёт кость и фиксирует зерно рандома.
        /// Если Зерно рандома не задано, то оно выбирается случайно.
        /// </summary>
        /// <returns> Экземпляр кости на основе выбранного или указанного ерна рандома. </returns>
        private IDice CreateRandomSeedAndExpDice()
        {
            IDice dice;
            if (DiceSeed == null)
            {
                var diceSeedFact = new Random().Next(int.MaxValue);
                DiceSeed = diceSeedFact;
                dice = new ExpDice(diceSeedFact);
            }
            else
            {
                dice = new ExpDice(DiceSeed.Value);
            }

            return dice;
        }

        private void RegisterClientServices(IServiceRegistry container)
        {
            container.Register<ISectorUiState, SectorUiState>(LightInjectWrapper.CreateScoped());
            container.Register<IInventoryState, InventoryState>(LightInjectWrapper.CreateScoped());
        }

        private void RegisterPlayerServices(IServiceRegistry container)
        {
            container.Register<IScoreManager, ScoreManager>(LightInjectWrapper.CreateSingleton());
            container.Register<HumanPlayer>(LightInjectWrapper.CreateSingleton());
            container.Register<IBotPlayer, BotPlayer>(LightInjectWrapper.CreateSingleton());
        }

        protected abstract void RegisterBot(IServiceRegistry container);
    }
}
