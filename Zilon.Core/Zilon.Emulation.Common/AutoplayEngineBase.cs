﻿using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Sdk;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Scoring;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Emulation.Common
{
    public interface IAutoplayContext
    {
        Task<bool> CheckNextIterationAsync();
    }

    public sealed class AutoplayContext : IAutoplayContext
    {
        private readonly IPerson _followedPerson;

        public AutoplayContext(IPerson followedPerson)
        {
            _followedPerson = followedPerson;
        }

        public async Task<bool> CheckNextIterationAsync()
        {
            return await Task.Run(() =>
            {
                return !_followedPerson.GetModule<ISurvivalModule>().IsDead;
            });
        }
    }


    public abstract class AutoplayEngineBase
    {
        private const int ITERATION_LIMIT = 4000;
        private readonly IGlobeInitializer _globeInitializer;

        protected AutoplayEngineBase(BotSettings botSettings,
            IGlobeInitializer globeInitializer)
        {
            BotSettings = botSettings;
            _globeInitializer = globeInitializer;
        }

        public IPlayerEventLogService PlayerEventLogService { get; set; }

        protected BotSettings BotSettings { get; }

        protected IServiceScope ServiceScope { get; set; }

        public async Task<IGlobe> CreateGlobeAsync()
        {
            // Create globe
            var globeInitializer = _globeInitializer;
            var globe = await globeInitializer.CreateGlobeAsync("intro").ConfigureAwait(false);
            return globe;
        }

        public async Task StartAsync(IGlobe globe, IAutoplayContext context)
        {
            if (globe is null)
            {
                throw new ArgumentNullException(nameof(globe));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var iterationCounter = 1;
            while (await context.CheckNextIterationAsync() && iterationCounter <= ITERATION_LIMIT)
            {
                for (var updateCounter = 0; updateCounter < GlobeMetrics.OneIterationLength; updateCounter++)
                {
                    try
                    {
                        await globe.UpdateAsync(CancellationToken.None).ConfigureAwait(false);
                    }
                    catch (ActorTaskExecutionException exception)
                    {
                        CatchActorTaskExecutionException(exception);
                    }
                    catch (AggregateException exception)
                    {
                        CatchException(exception.InnerException);
                        throw;
                    }
                }

                iterationCounter++;
            }

            if (iterationCounter >= ITERATION_LIMIT)
            {
                if (PlayerEventLogService != null)
                {
                    var endOfLifeEvent = new EndOfLifeEvent();
                    PlayerEventLogService.Log(endOfLifeEvent);
                }
            }

            ProcessEnd();
        }

        protected abstract void CatchActorTaskExecutionException(ActorTaskExecutionException exception);

        protected virtual void CatchException(Exception exception)
        {
            Console.WriteLine(exception);
        }

        protected abstract void ConfigBotAux();

        protected abstract void ProcessEnd();

        protected abstract void ProcessSectorExit();
    }
}